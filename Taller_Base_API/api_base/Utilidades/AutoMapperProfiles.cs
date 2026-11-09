using AutoMapper;
using api.DTOs;
using api.Entidades;

namespace api.Utilidades
{
    // Mapeos de LECTURA (entidad -> DTO) para todos los módulos y mapeos de Géneros.
    // Los mapeos de CREACIÓN (DTO -> entidad) los escribe cada grupo en su propio Profile.
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            ConfigurarMapeoGeneros();
            ConfigurarMapeosDeLectura();
        }

        private void ConfigurarMapeoGeneros()
        {
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();
        }

        private void ConfigurarMapeosDeLectura()
        {
            CreateMap<Actor, ActorDTO>();
            CreateMap<Cine, CineDTO>();
            CreateMap<Productora, ProductoraDTO>();
            CreateMap<Pelicula, PeliculaDTO>();
            CreateMap<Actor, ActorPeliculaDTO>();
        }
    }
}
