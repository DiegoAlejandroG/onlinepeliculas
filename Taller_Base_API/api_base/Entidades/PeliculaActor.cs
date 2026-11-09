namespace api.Entidades
{
    public class PeliculaActor
    {
        public int PeliculaId { get; set; }
        public int ActorId { get; set; }
        public string Personaje { get; set; } = string.Empty;
        public int Orden { get; set; }
        public Pelicula Pelicula { get; set; } = null!;
        public Actor Actor { get; set; } = null!;
    }
}
