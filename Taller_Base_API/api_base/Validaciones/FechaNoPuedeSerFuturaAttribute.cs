using System.ComponentModel.DataAnnotations;

namespace api.Validaciones
{
    public class FechaNoPuedeSerFuturaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly fecha && fecha > DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult("La fecha no puede ser del futuro");
            }

            return ValidationResult.Success;
        }
    }
}
