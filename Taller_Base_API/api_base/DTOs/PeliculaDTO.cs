using api.Entidades;

namespace api.DTOs
{
    public class PeliculaDTO : IId
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public string? Trailer { get; set; }
        public DateOnly FechaLanzamiento { get; set; }
        public string? Poster { get; set; }
    }
}
