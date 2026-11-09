namespace api.DTOs
{
    // Forma en que el frontend ve a un actor dentro de una película (id, nombre, personaje y foto).
    public class ActorPeliculaDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public string Personaje { get; set; } = string.Empty;
        public string? Foto { get; set; }
    }
}
