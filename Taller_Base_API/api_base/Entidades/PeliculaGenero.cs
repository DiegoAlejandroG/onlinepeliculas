namespace api.Entidades
{
    public class PeliculaGenero
    {
        public int PeliculaId { get; set; }
        public int GeneroId { get; set; }
        public Pelicula Pelicula { get; set; } = null!;
        public Genero Genero { get; set; } = null!;
    }
}
