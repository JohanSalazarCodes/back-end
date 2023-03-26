using AutoMapper;
using back_end.DTOs;
using back_end.Entidades;
using back_end.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class GenerosController : ControllerBase
    {

        private readonly ILogger<GenerosController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GenerosController(
            ILogger<GenerosController> logger, ApplicationDbContext context, IMapper mapper) 
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        //[ResponseCache(Duration = 60)]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<List<GeneroDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = _context.Genero.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var generos = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();

            return _mapper.Map<List<GeneroDTO>>(generos);
        }

        [HttpGet("todos")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GeneroDTO>>> Todos()
        {
            var generos = await _context.Genero.ToListAsync();
            return _mapper.Map<List<GeneroDTO>>(generos);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<GeneroDTO>> Get(int Id)
        {
            var genero = await _context.Genero.FirstOrDefaultAsync(x => x.Id == Id);
            if (genero == null)
            {
                return NotFound();
            }

            return _mapper.Map<GeneroDTO> (genero);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = _mapper.Map<Genero>(generoCreacionDTO);
            _context.Add(genero);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = await _context.Genero.FirstOrDefaultAsync(x => x.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            genero = _mapper.Map(generoCreacionDTO, genero);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe  = await _context.Genero.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();  
            }

            _context.Remove(new Genero { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
