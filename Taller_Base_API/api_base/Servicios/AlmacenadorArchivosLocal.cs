namespace api.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        private string RaizWeb => env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var carpeta = Path.Combine(RaizWeb, contenedor);

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            var rutaFisica = Path.Combine(carpeta, nombreArchivo);
            await using (var flujo = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(flujo);
            }

            var solicitud = httpContextAccessor.HttpContext!.Request;
            return $"{solicitud.Scheme}://{solicitud.Host}/{contenedor}/{nombreArchivo}";
        }

        public async Task<string> Editar(string? ruta, string contenedor, IFormFile archivo)
        {
            await Borrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.GetFileName(ruta);
            var rutaFisica = Path.Combine(RaizWeb, contenedor, nombreArchivo);

            if (File.Exists(rutaFisica))
            {
                File.Delete(rutaFisica);
            }

            return Task.CompletedTask;
        }
    }
}
