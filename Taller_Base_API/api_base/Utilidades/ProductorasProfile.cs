using api.DTOs;
using api.Entidades;
using AutoMapper;

namespace api.Utilidades
{
    public class ProductorasProfile : Profile
    {
        public ProductorasProfile()
        {
            CreateMap<ProductoraCreacionDTO, Productora>()
                .ForMember(productora => productora.Logo, opciones => opciones.Ignore());
        }
    }
}