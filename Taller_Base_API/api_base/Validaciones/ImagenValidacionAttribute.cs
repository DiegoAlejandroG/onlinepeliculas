using System.ComponentModel.DataAnnotations;

namespace api.Validaciones
{
    // Valida que el archivo recibido sea una imagen (jpg o png) y que no pese más de los MB indicados.
    public class ImagenValidacionAttribute : ValidationAttribute
    {
        private readonly int pesoMaximoEnMegabytes;
        private static readonly string[] tiposPermitidos = ["image/jpeg", "image/png"];

        public ImagenValidacionAttribute(int pesoMaximoEnMegabytes = 4)
        {
            this.pesoMaximoEnMegabytes = pesoMaximoEnMegabytes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile archivo)
            {
                return ValidationResult.Success;
            }

            if (archivo.Length > pesoMaximoEnMegabytes * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no debe ser mayor a {pesoMaximoEnMegabytes} MB");
            }

            if (!tiposPermitidos.Contains(archivo.ContentType))
            {
                return new ValidationResult("El archivo debe ser una imagen jpg o png");
            }

            return ValidationResult.Success;
        }
    }
}
