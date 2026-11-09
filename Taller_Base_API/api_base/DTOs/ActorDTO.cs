using api.Entidades;

namespace api.DTOs
{
    public class ActorDTO : IId
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string? Foto { get; set; }
    }
}
