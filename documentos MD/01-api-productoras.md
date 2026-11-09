# 01 · API de Productoras (Equipo C) — Guía para el agente

> Archivo 1 de 3. Orden de uso: `01-api-productoras.md` → `02-frontend-productoras.md` → `03-verificacion-y-entrega.md`.

## 0. Rol y alcance

Eres un agente de programación que ayuda al **Equipo C (Productoras)** del Taller Final "Películas OnLine TdeA" (Construcción de Software IV, TdeA). Este archivo cubre **solo el backend** (ASP.NET Core + EF Core).

### Archivos que SÍ puedes crear (únicos permitidos)

```
api_base/DTOs/ProductoraCreacionDTO.cs
api_base/Utilidades/ProductorasProfile.cs
api_base/Controllers/ProductorasController.cs
```

### Archivos que NO puedes modificar (contrato común)

Entidades (`Entidades/`), `ApplicationDbContext.cs`, `Program.cs`, DTOs de lectura (`ProductoraDTO`, etc.), `AutoMapperProfiles.cs`, `Servicios/`, `Validaciones/`, `Utilidades/` del base, `Migrations/` (no se entrega). Si crees que algo del base está mal, **detente y avisa al usuario** para que consulte al docente.

---

## 1. Reglas de negocio de Productoras (fuente de verdad)

| # | Regla | Dónde se hace cumplir |
|---|---|---|
| R1 | `nombre`: obligatorio, máx. 100 caracteres, primera letra mayúscula | **API (DTO)** y frontend |
| R2 | `fechaFundacion`: obligatoria, no futura y no anterior al 1 de enero de 1900 | **API (DTO)** y frontend |
| R3 | `sitioWeb`: obligatorio, máx. 300 caracteres, formato URL (`El campo {0} debe ser una URL válida`) | **API (DTO)** y frontend |
| R4 | `logo`: opcional; si viene, jpg o png de 4 MB o menos | **API (DTO)** y frontend |
| R5 | Caché de salida con etiqueta `"productoras"`; POST, PUT y DELETE la invalidan | **API** |
| R6 | PUT sin logo nuevo conserva el actual; con logo, reemplaza y borra el archivo viejo | **API** |
| R7 | DELETE borra también el archivo del logo | **API** |

**Advertencias derivadas del documento de reglas de negocio del proyecto:**

- Las validaciones del frontend **no** protegen al API: cualquiera puede llamar al API sin la interfaz. Por eso R1–R4 **deben** estar en el DTO.
- Los atributos reutilizables (`FechaNoPuedeSerFutura`, `FechaNoAnteriorA1900`, `ImagenValidacion`) existen en `Validaciones/`, pero **no están aplicados** a ningún DTO del módulo hasta que tú los apliques en `ProductoraCreacionDTO`.
- **No inventes reglas** que el negocio no define: no hay unicidad de nombre, no hay relación Productora–Película, no hay límites de longitud distintos a los de la tabla.

---

## 2. Protocolo de ciclo de respuesta (OBLIGATORIO en cada paso)

Cada paso de este archivo se ejecuta con este ciclo. **No avances al siguiente paso sin un "OK" explícito del usuario.**

```
1. ANUNCIAR   → "Paso X: <nombre>. Objetivo: <una línea>."
2. EJECUTAR   → Haz solo lo que pide el paso. Nada extra.
3. VERIFICAR  → Corre el comando o la comprobación indicada en el paso.
4. REPORTAR   → Usa EXACTAMENTE el formato de abajo.
5. ESPERAR    → Pregunta: "¿Confirmas el Paso X? Responde OK o CORREGIR: <qué>."
6. DECIDIR    → OK → siguiente paso.  CORREGIR → repite el ciclo del mismo paso e indica qué cambiaste.
```

### Formato de reporte (copia esta plantilla)

```
## Reporte — Paso X: <nombre>
**Estado:** ✅ Completo | ⚠️ Parcial | ❌ Bloqueado
**Archivos creados/modificados:** <ruta> (creado|modificado) …
**Comandos ejecutados y salida (literal, resumida si es larga):**
  $ <comando>
  <salida>
**Criterios del paso:**
  - [✅|❌] <criterio 1>
  - [✅|❌] <criterio 2>
**Reglas de negocio cubiertas:** R1, R2 …
**Desviaciones, supuestos o dudas:** <o "ninguna">
**¿Confirmas el Paso X? Responde OK o CORREGIR: <qué>.**
```

### Reglas del agente

1. **Nunca marques ✅ algo que no verificaste.** Si no puedes ejecutar un comando (sin acceso, sin base de datos), di "no verificado" y explica cómo lo verificará el usuario.
2. Si lo que encuentras en el base **difiere** de lo que dice esta guía (nombres de propiedades, firmas del almacenador, namespaces), **no adivines**: repórtalo en "Desviaciones" y adapta el código a lo real, mostrando la diferencia.
3. Si un paso exige modificar un archivo del contrato común, **detente** (estado ❌ Bloqueado) y pregunta al usuario.
4. Mantén el estilo del proyecto (nombres en español, `GenerosController` como patrón).
5. Un cambio por paso; no mezcles pasos.

---

## 3. Pasos

### Paso 0 — Reconocimiento del proyecto base (solo lectura)

**Objetivo:** conocer los nombres reales antes de escribir código.

Lee (sin modificar) y anota lo relevante:

1. `Controllers/GenerosController.cs` y `Controllers/CustomBaseController.cs` → patrón de constructor, `[OutputCache]`, `EvictByTagAsync`, rutas con nombre (`ObtenerXPorId`).
2. `Entidades/Productora.cs` → nombres exactos: `Nombre`, `FechaFundacion` (¿`DateOnly`?), `SitioWeb`, `Logo`.
3. `DTOs/ProductoraDTO.cs` y el mapeo de lectura en `Utilidades/AutoMapperProfiles.cs` (Productora → ProductoraDTO ya existe; no tocar).
4. `Servicios/IAlmacenadorArchivos.cs` → firmas reales de `Almacenar`, `Editar`, `Borrar` (orden de parámetros: ruta/contenedor/archivo).
5. `Validaciones/` → nombres exactos de los atributos: `PrimeraLetraMayuscula`, `FechaNoPuedeSerFutura`, `FechaNoAnteriorA1900`, `ImagenValidacion`.
6. `Utilidades/` → extensiones `InsertarParametrosPaginacionEnCabecera` y `Paginar`; `PaginacionDTO`.
7. `Program.cs` → que registre almacenador, `UseStaticFiles`, `AddMaps`, output cache.
8. `ApplicationDbContext.cs` → nombre del `DbSet` (probablemente `Productoras`).

**Criterios:** tabla de hallazgos con namespace del proyecto, firmas del almacenador y nombre del DbSet. **Ningún archivo modificado.**

---

### Paso 1 — Preparar y ejecutar el API base

**Objetivo:** confirmar que el entorno funciona antes de programar.

1. Revisar la cadena de conexión en `appsettings.Development.json` (por defecto LocalDB, base `PeliculasTaller`).
2. Ejecutar:
   ```bash
   dotnet restore
   dotnet ef migrations add ModeloInicial
   dotnet ef database update
   dotnet watch run --launch-profile https
   ```
3. En Swagger: `GET /api/generos` responde **200** con lista (vacía o con datos).
4. (Opcional) Ejecutar `Scripts/datos_prueba.sql`.

**Criterios:** el API arranca; Swagger abre; la migración se creó **localmente** y **no** se incluirá en la entrega.

---

### Paso 2 — `DTOs/ProductoraCreacionDTO.cs`

**Objetivo:** DTO de creación/edición con todas las validaciones (R1–R4).

Referencia (ajusta namespace y nombres a lo encontrado en el Paso 0):

```csharp
using System.ComponentModel.DataAnnotations;

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

    [ImagenValidacion(4)] // jpg o png, hasta 4 MB
    public IFormFile? Logo { get; set; }
}
```

**Notas:**

- `FechaFundacion` es `DateOnly` no nulo: si no se envía, llega `0001-01-01` y la regla de 1900 la rechaza (la fecha queda efectivamente obligatoria — R2).
- Mensajes **exactos** (se califican): `La primera letra debe ser mayúscula`, `La fecha no puede ser del futuro`, `La fecha no puede ser anterior a 1900`, `El campo SitioWeb debe ser una URL válida`, `El archivo debe ser una imagen jpg o png`.

**Criterios:** compila; cada propiedad lleva sus atributos; `Logo` es `IFormFile?`.

---

### Paso 3 — `Utilidades/ProductorasProfile.cs`

**Objetivo:** mapeo de **creación** (DTO → entidad). El de lectura ya existe.

```csharp
using AutoMapper;

public class ProductorasProfile : Profile   // debe ser public y heredar de Profile
{
    public ProductorasProfile()
    {
        CreateMap<ProductoraCreacionDTO, Productora>()
            .ForMember(p => p.Logo, opciones => opciones.Ignore());
    }
}
```

**Criterios:** clase `public`; ignora `Logo` (AutoMapper no convierte `IFormFile` a texto; el controlador asigna la URL con el almacenador). No toca `AutoMapperProfiles.cs`.

---

### Paso 4 — `Controllers/ProductorasController.cs`

Hazlo en **tres sub-ciclos** (4A, 4B, 4C), cada uno con su propio reporte y confirmación.

**Estructura base** (adapta firmas a lo real del Paso 0):

```csharp
[ApiController]
[Route("api/productoras")]
public class ProductorasController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IOutputCacheStore outputCacheStore;
    private readonly IAlmacenadorArchivos almacenadorArchivos;
    private const string cacheTag = "productoras";
    private const string contenedor = "productoras";

    public ProductorasController(ApplicationDbContext context, IMapper mapper,
        IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
    { /* asignar campos */ }
}
```

#### 4A — Lecturas (GET)

| Ruta | Comportamiento |
|---|---|
| `GET /api/productoras?pagina=1&recordsPorPagina=10` | 200 con `List<ProductoraDTO>` **ordenada por nombre**; cabecera `cantidad-total-registros`; `[OutputCache(Tags = [cacheTag])]` |
| `GET /api/productoras/{id:int}` | 200 con `ProductoraDTO`, o **404**; `[HttpGet("{id:int}", Name = "ObtenerProductoraPorId")]`; `[OutputCache(Tags = [cacheTag])]` |

Orden clave del GET paginado: `AsQueryable()` → `InsertarParametrosPaginacionEnCabecera` (**antes** de paginar) → `OrderBy(Nombre)` → `Paginar` → mapear a `ProductoraDTO`.

#### 4B — Escritura (POST, PUT) con `[FromForm]`

- **POST:** mapear DTO → entidad; si `Logo is not null`, `entidad.Logo = await almacenadorArchivos.Almacenar(contenedor, dto.Logo)`; `Add` + `SaveChangesAsync`; `EvictByTagAsync(cacheTag, default)`; `CreatedAtRoute("ObtenerProductoraPorId", new { id }, mapper.Map<ProductoraDTO>(entidad))` → **201** con `Location`.
- **PUT `{id:int}`:** buscar; si no existe → **404**; `mapper.Map(dto, productoraDB)`; si `Logo is not null` → `productoraDB.Logo = await almacenadorArchivos.Editar(productoraDB.Logo, contenedor, dto.Logo)` (reemplaza y borra el viejo); si no viene logo, **no tocar** `Logo` (R6); `SaveChangesAsync`; evict; **204**.

#### 4C — Borrado (DELETE)

- `DELETE {id:int}`: buscar la entidad (para conocer la ruta del logo); si no existe → **404**; `Remove`; `SaveChangesAsync`; si tenía logo → `almacenadorArchivos.Borrar(logo, contenedor)`; evict; **204** (R7).

**Criterios de todo el paso 4:** `dotnet build` sin errores; códigos 201/204/400/404 correctos; los 5 endpoints existen con las rutas exactas; ningún GET sin `OutputCache` y ninguna escritura sin `EvictByTagAsync`.

---

### Paso 5 — Compilar y probar en Swagger

1. `dotnet build` → **0 errores**.
2. Ejecutar el API y probar en Swagger (cada fila es una verificación con evidencia):

| # | Acción | Resultado esperado |
|---|---|---|
| 1 | POST con nombre `pixar` | 400: `La primera letra debe ser mayúscula` |
| 2 | POST con `fechaFundacion` = `1850-01-01` | 400: `La fecha no puede ser anterior a 1900` |
| 3 | POST con `sitioWeb` = `hola` | 400: `El campo SitioWeb debe ser una URL válida` |
| 4 | POST `Pixar`, `1986-02-03`, `https://www.pixar.com`, con logo png | 201 + `Location`; archivo en `wwwroot/productoras` |
| 5 | PUT sin logo | 204; el logo sigue igual |
| 6 | PUT con otro logo | 204; el archivo anterior ya no existe |
| 7 | DELETE | 204; se borra el archivo del logo |
| 8 | GET `/api/productoras/9999` | 404 |
| 9 | GET paginado tras crear/editar | La lista refleja el cambio (caché invalidado) |

**Errores frecuentes del API:** `Missing map from X to Y` → el Profile no es `public` o falta el `CreateMap`; `400 The Logo field is required` → usaste `[FromBody]` en vez de `[FromForm]`; lista desactualizada → falta `EvictByTagAsync` con la misma etiqueta.

---

## 4. Cierre del archivo 01

Entrega al usuario un **resumen final** con: archivos creados, resultado de `dotnet build`, tabla de pruebas del Paso 5 con ✅/❌ y reglas R1–R7 cubiertas. Termina con:

> "Archivo 01 completado. ¿Confirmas con OK para continuar con `02-frontend-productoras.md`?"
