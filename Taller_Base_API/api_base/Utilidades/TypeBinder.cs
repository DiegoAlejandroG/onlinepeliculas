using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace api.Utilidades
{
    // En un formulario multipart/form-data todo llega como texto. Este binder recibe un valor
    // como "[1,2,3]" y lo convierte en una List<int> (o cualquier otro tipo T que se indique).
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var proveedorDeValores = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            if (proveedorDeValores == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var valorDeserializado = JsonSerializer.Deserialize<T>(proveedorDeValores.FirstValue!,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                bindingContext.Result = ModelBindingResult.Success(valorDeserializado);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(nombrePropiedad,
                    $"El valor de {nombrePropiedad} no tiene el formato esperado");
            }

            return Task.CompletedTask;
        }
    }
}
