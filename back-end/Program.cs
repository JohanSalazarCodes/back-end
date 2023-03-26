using AutoMapper;
using back_end;
using back_end.Filtros;
using back_end.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddResponseCaching();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(FiltroDeExcepcion));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"), 
            sqlServer => sqlServer.UseNetTopologySuite()));

builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => 
    {
        policy.WithOrigins(builder.Configuration.GetValue<string>("frontend_url")).AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(provider =>
                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactory));
                }).CreateMapper());

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddAuthorization(opciones =>{
    opciones.AddPolicy("EsAdmin", policy => policy.RequireClaim("role", "admin"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience= false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
        ClockSkew = TimeSpan.Zero
    });;



builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorAzureStorage>();
//builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    using (var swapStream = new MemoryStream())
//    {
//        var respuestaOriginal = context.Response.Body;
//        context.Response.Body = swapStream;

//        await next.Invoke();

//        swapStream.Seek(0, SeekOrigin.Begin);
//        string respuesta = new StreamReader(swapStream).ReadToEnd();
//        swapStream.Seek(0, SeekOrigin.Begin);

//        await swapStream.CopyToAsync(respuestaOriginal);
//        context.Response.Body = respuestaOriginal;

//        app.Logger.LogInformation(respuesta); 

//    }
//});

//app.Map("/mapa1", (app) =>
//{
//    app.Run(async context =>
//    {
//        await context.Response.WriteAsync("Estoy interceptando el pipeline");
//    });
//});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
//app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
