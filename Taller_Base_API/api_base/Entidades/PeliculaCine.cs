namespace api.Entidades
{
    public class PeliculaCine
    {
        public int PeliculaId { get; set; }
        public int CineId { get; set; }
        public Pelicula Pelicula { get; set; } = null!;
        public Cine Cine { get; set; } = null!;
    }
}
