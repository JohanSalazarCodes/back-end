using AutoMapper;
using back_end.DTOs;
using back_end.Entidades;
using back_end.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly string contenedor = "actores";
        public ActoresController( ApplicationDbContext context, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = _context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var actores = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();

            return _mapper.Map<List<ActorDTO>>(actores);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int Id)
        {
            var actor = await _context.Actores.FirstOrDefaultAsync(x => x.Id == Id);
            if (actor == null)
            {
                return NotFound();
            }

            return _mapper.Map<ActorDTO>(actor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await _almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionDTO.Foto);
            }

            _context.Add(actor);
            await _context.SaveChangesAsync();  
            return NoContent();
        }

        [HttpPost("buscarPorNombre")]
        public async Task<ActionResult<List<PeliculaActorDTO>>> BuscarPorNombre([FromBody] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) { return new List<PeliculaActorDTO>(); }
            return await _context.Actores
                .Where(x => x.Nombre.Contains(nombre))
                .Select(x => new PeliculaActorDTO { Id = x.Id, Nombre = x.Nombre, Foto = x.Foto })
                .Take(5)
                .ToListAsync();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = await _context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            actor = _mapper.Map(actorCreacionDTO, actor);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await _almacenadorArchivos.EditarArchivo(contenedor, actorCreacionDTO.Foto, actor.Foto);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await _context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Remove(new Actor { Id = id });
            await _context.SaveChangesAsync();

            await _almacenadorArchivos.BorrarArchivo(actor.Foto, contenedor);

            return NoContent();
        }
    }
}
