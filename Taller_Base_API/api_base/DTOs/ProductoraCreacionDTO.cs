using api.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class ProductoraCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        [FechaNoPuedeSerFutura]
        [FechaNoAnteriorA1900]
        public DateOnly FechaFundacion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(300, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [Url(ErrorMessage = "El campo {0} debe ser una URL válida")]
        public required string SitioWeb { get; set; }

        [ImagenValidacion(4)]
        public IFormFile? Logo { get; set; }
    }
}