using System.ComponentModel.DataAnnotations;

namespace api.Validaciones
{
    public class FechaNoAnteriorA1900Attribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly fecha && fecha < new DateOnly(1900, 1, 1))
            {
                return new ValidationResult("La fecha no puede ser anterior a 1900");
            }

            return ValidationResult.Success;
        }
    }
}
