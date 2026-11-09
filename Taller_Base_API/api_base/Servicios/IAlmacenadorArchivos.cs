namespace api.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<string> Almacenar(string contenedor, IFormFile archivo);
        Task<string> Editar(string? ruta, string contenedor, IFormFile archivo);
        Task Borrar(string? ruta, string contenedor);
    }
}
