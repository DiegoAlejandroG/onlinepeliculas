using api.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace api.Entidades
{
    public class Productora : IId
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        public DateOnly FechaFundacion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(300, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string SitioWeb { get; set; }

        public string? Logo { get; set; }
    }
}
