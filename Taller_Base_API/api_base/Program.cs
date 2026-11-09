using api;
using api.Servicios;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AddMaps busca TODAS las clases que heredan de Profile en el proyecto:
// cada grupo agrega su propio archivo de mapeos sin tocar los de los demás.
builder.Services.AddSingleton(proveedor => new MapperConfiguration(configuracion =>
{
    configuracion.AddMaps(typeof(Program).Assembly);
}).CreateMapper());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!.Split(",");

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
            .WithExposedHeaders("cantidad-total-registros");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Sirve las imágenes guardadas en wwwroot (actores, productoras y películas).
app.UseStaticFiles();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseOutputCache();


app.Run();
