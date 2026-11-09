# Reglas de negocio — OnlinePeliculas

## Alcance

Este documento describe las reglas que se observan en el código actual del proyecto, compuesto por una API ASP.NET Core y una interfaz React. Distingue entre reglas validadas por la API, validaciones de formularios del frontend y propiedades que solo están representadas en el modelo de datos. No se consideran reglas implementadas aquellas que el código no valida.

## Dominio y relaciones

- El sistema modela películas, géneros, actores, cines y productoras.
- Una película puede relacionarse con varios géneros y varios cines; las tablas de unión evitan duplicar la misma pareja película-género o película-cine.
- Una película puede relacionarse con varios actores. La relación guarda además el personaje interpretado y el orden del actor en el reparto.
- Una productora tiene nombre, fecha de fundación, sitio web y un logo opcional. En el modelo actual no existe una relación entre productoras y películas.
- Los campos de imagen de películas y actores (póster y foto) son opcionales en las entidades. Logo también es opcional.

Las relaciones de película, género, cine y actor están definidas en las entidades y en [ApplicationDbContext.cs](../Taller_Base_API/api_base/ApplicationDbContext.cs), pero eso por sí solo no implica que exista una operación de API para administrarlas.

## Reglas validadas por la API

La API contiene endpoints activos para géneros en `api/generos`:

- Al crear o actualizar un género, el nombre es obligatorio, admite hasta 50 caracteres y su primera letra debe ser mayúscula.
- Se puede consultar el listado paginado y ordenado por nombre, consultar todos los géneros ordenados por nombre, y consultar un género por identificador.
- Si el identificador consultado, actualizado o eliminado no existe, la API responde con `404`. Una actualización o eliminación exitosa responde con `204`; una creación responde con `201`.
- Las respuestas de lectura se almacenan en caché; al crear, actualizar o eliminar géneros se invalida la caché asociada a ese recurso.

Estas reglas se implementan en [GenerosController.cs](../Taller_Base_API/api_base/Controllers/GenerosController.cs), [GeneroCreacionDTO.cs](../Taller_Base_API/api_base/DTOs/GeneroCreacionDTO.cs) y [CustomBaseController.cs](../Taller_Base_API/api_base/Controllers/CustomBaseController.cs).

## Validaciones presentes en los formularios

Las siguientes validaciones están implementadas en el frontend. No deben interpretarse como validaciones garantizadas por la API:

### Géneros

- El formulario solicita un nombre y aplica las reglas de longitud y mayúscula inicial descritas para el género.

### Productoras

- Nombre obligatorio con inicial mayúscula.
- Fecha de fundación obligatoria, no anterior al 1 de enero de 1900 y no futura.
- Sitio web obligatorio y con formato de URL válido.

### Actores

- Nombre obligatorio con inicial mayúscula.
- Fecha de nacimiento obligatoria y no futura.

### Cines

- Nombre obligatorio con inicial mayúscula.
- El formulario requiere latitud y longitud para completar el registro; no establece límites geográficos para esas coordenadas.

### Películas

- Título y fecha de lanzamiento son obligatorios en el formulario.
- El usuario puede asociar géneros, cines y actores; para cada actor asociado puede indicar el personaje.
- El formulario no declara obligatoria una selección de géneros, cines o actores.

Las validaciones y campos de estos formularios están en [Validaciones.ts](../Taller_Base_Frontend/frontend_base/src/validaciones/Validaciones.ts), [FormularioProductora.tsx](../Taller_Base_Frontend/frontend_base/src/features/productoras/componentes/FormularioProductora.tsx), [FormularioActor.tsx](../Taller_Base_Frontend/frontend_base/src/features/actores/componentes/FormularioActor.tsx), [FormularioCine.tsx](../Taller_Base_Frontend/frontend_base/src/features/cines/componentes/FormularioCine.tsx) y [FormularioPelicula.tsx](../Taller_Base_Frontend/frontend_base/src/features/peliculas/componentes/FormularioPelicula.tsx).

## Restricciones representadas en las entidades

Las entidades también declaran restricciones de datos que sirven como parte del modelo, pero no todas tienen un endpoint de API que actualmente las haga cumplir:

| Entidad | Campo | Restricción declarada |
|---|---|---|
| Género | Nombre | Obligatorio, máximo 50 caracteres, inicial mayúscula |
| Productora | Nombre | Obligatorio, máximo 100 caracteres, inicial mayúscula |
| Productora | Sitio web | Obligatorio, máximo 300 caracteres |
| Película | Título | Obligatorio, máximo 150 caracteres |
| Película | Tráiler | Opcional, máximo 500 caracteres |
| Actor | Nombre | Obligatorio, máximo 150 caracteres, inicial mayúscula |
| Cine | Nombre | Obligatorio, máximo 75 caracteres, inicial mayúscula |
| Actor / Película | Fecha | Fecha de nacimiento y de lanzamiento están representadas como fechas sin hora |

Las propiedades de las entidades están en el directorio [Entidades](../Taller_Base_API/api_base/Entidades). Las tablas de unión película-género, película-cine y película-actor usan claves compuestas en lugar de un identificador propio, según [ApplicationDbContext.cs](../Taller_Base_API/api_base/ApplicationDbContext.cs).

## Límites de la implementación actual

- En el backend se encontraron rutas de controlador para géneros; los formularios y rutas de interfaz para películas, actores, cines y productoras no equivalen a endpoints disponibles en la API.
- Los atributos reutilizables para impedir fechas anteriores a 1900, fechas futuras e imágenes no aparecen aplicados a los DTOs o entidades revisados. Por ello, no se documentan como restricciones garantizadas por el servidor.
- La validación de formatos de fecha, URL, mayúscula inicial y campos requeridos en los formularios puede eludirse si se envían datos sin usar la interfaz.
- El modelo no declara unicidad para nombres de géneros, actores, cines o productoras, ni límites de latitud/longitud. No se asume que esos valores sean únicos o geográficamente válidos.
- La fecha de lanzamiento de una película puede ser futura según los datos de ejemplo y el formulario; no se restringe a estrenos ya ocurridos.

## Referencias principales

- [API: controlador de géneros](../Taller_Base_API/api_base/Controllers/GenerosController.cs)
- [API: entidades](../Taller_Base_API/api_base/Entidades)
- [API: validaciones](../Taller_Base_API/api_base/Validaciones)
- [Frontend: formularios y validaciones](../Taller_Base_Frontend/frontend_base/src)
- [Datos de prueba](../Taller_Base_API/api_base/Scripts/datos_prueba.sql)
