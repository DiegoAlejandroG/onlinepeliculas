using api.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace api.Entidades
{
    public class Actor : IId
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        public DateOnly FechaNacimiento { get; set; }

        public string? Foto { get; set; }

        public List<PeliculaActor> PeliculasActores { get; set; } = [];
    }
}
