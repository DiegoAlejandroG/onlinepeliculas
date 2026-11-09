using api.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace api.Entidades
{
    public class Cine : IId
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(75, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public List<PeliculaCine> PeliculasCines { get; set; } = [];
    }
}
