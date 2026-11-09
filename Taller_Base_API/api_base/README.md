# API Cartelera de Películas (.NET 10)

## Puesta en marcha
1. `dotnet restore`
2. Revisa la cadena de conexión en `appsettings.Development.json` (por defecto LocalDB, base `PeliculasTaller`).
3. Crear la base de datos (la primera vez):
   - `dotnet tool install --global dotnet-ef` (solo si aún no lo tienes)
   - `dotnet ef migrations add ModeloInicial`
   - `dotnet ef database update`
4. (Opcional) cargar datos de prueba: ejecuta `Scripts/datos_prueba.sql` en tu cliente de SQL Server.
5. `dotnet watch run --launch-profile https` → Swagger en https://localhost:7256/swagger

Las imágenes subidas se guardan en `wwwroot/<contenedor>/` y se sirven en `https://localhost:7256/<contenedor>/<archivo>`.
