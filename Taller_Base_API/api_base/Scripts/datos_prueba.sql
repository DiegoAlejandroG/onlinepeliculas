-- Datos de prueba para el taller final (SQL Server).
-- Ejecútalo DESPUÉS de aplicar la migración (dotnet ef database update), sobre la base PeliculasTaller.
-- Es seguro ejecutarlo varias veces: no inserta nada si ya hay películas.
USE PeliculasTaller;
GO

IF NOT EXISTS (SELECT 1 FROM Peliculas)
BEGIN
    -- Géneros
    INSERT INTO Generos (Nombre)
    SELECT n FROM (VALUES (N'Acción'), (N'Aventura'), (N'Ciencia ficción'), (N'Comedia'), (N'Drama')) AS t(n)
    WHERE NOT EXISTS (SELECT 1 FROM Generos g WHERE g.Nombre = t.n);

    -- Cines
    INSERT INTO Cines (Nombre, Latitud, Longitud) VALUES
        (N'Viva Envigado', 6.1760, -75.5920),
        (N'Premier Las Américas', 6.2380, -75.5970),
        (N'Parque Fabricato', 6.3330, -75.5560);

    -- Actores
    INSERT INTO Actores (Nombre, FechaNacimiento, Foto) VALUES
        (N'Tom Holland', '1996-06-01', NULL),
        (N'Zendaya', '1996-09-01', NULL),
        (N'Tom Hanks', '1956-07-09', NULL);

    -- Películas (las dos primeras ya están en cines; la tercera es un próximo estreno)
    INSERT INTO Peliculas (Titulo, Trailer, FechaLanzamiento, Poster) VALUES
        (N'Spider-Man: Lejos de casa', NULL, '2019-07-02', NULL),
        (N'Toy Story', NULL, '1995-11-22', NULL),
        (N'Aventura Futura', NULL, CAST(DATEADD(DAY, 60, GETDATE()) AS date), NULL);

    -- Relaciones
    INSERT INTO PeliculasGeneros (PeliculaId, GeneroId)
    SELECT p.Id, g.Id FROM Peliculas p, Generos g
    WHERE (p.Titulo = N'Spider-Man: Lejos de casa' AND g.Nombre IN (N'Acción', N'Aventura'))
       OR (p.Titulo = N'Toy Story' AND g.Nombre IN (N'Comedia', N'Aventura'))
       OR (p.Titulo = N'Aventura Futura' AND g.Nombre IN (N'Ciencia ficción'));

    INSERT INTO PeliculasCines (PeliculaId, CineId)
    SELECT p.Id, c.Id FROM Peliculas p, Cines c
    WHERE (p.Titulo = N'Spider-Man: Lejos de casa' AND c.Nombre IN (N'Viva Envigado', N'Premier Las Américas'))
       OR (p.Titulo = N'Toy Story' AND c.Nombre = N'Parque Fabricato');

    INSERT INTO PeliculasActores (PeliculaId, ActorId, Personaje, Orden)
    SELECT p.Id, a.Id, N'Peter Parker', 0 FROM Peliculas p, Actores a
    WHERE p.Titulo = N'Spider-Man: Lejos de casa' AND a.Nombre = N'Tom Holland';
    INSERT INTO PeliculasActores (PeliculaId, ActorId, Personaje, Orden)
    SELECT p.Id, a.Id, N'MJ', 1 FROM Peliculas p, Actores a
    WHERE p.Titulo = N'Spider-Man: Lejos de casa' AND a.Nombre = N'Zendaya';
    INSERT INTO PeliculasActores (PeliculaId, ActorId, Personaje, Orden)
    SELECT p.Id, a.Id, N'Woody (voz)', 0 FROM Peliculas p, Actores a
    WHERE p.Titulo = N'Toy Story' AND a.Nombre = N'Tom Hanks';
END
GO
