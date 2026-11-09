using System.ComponentModel.DataAnnotations;

namespace api.Entidades
{
    public class Pelicula : IId
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Titulo { get; set; }

        [StringLength(500, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public string? Trailer { get; set; }

        public DateOnly FechaLanzamiento { get; set; }

        public string? Poster { get; set; }

        public List<PeliculaGenero> PeliculasGeneros { get; set; } = [];
        public List<PeliculaCine> PeliculasCines { get; set; } = [];
        public List<PeliculaActor> PeliculasActores { get; set; } = [];
    }
}
