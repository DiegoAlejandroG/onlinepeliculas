# 03 · Verificación, integración y entrega (Equipo C) — Guía para el agente

> Archivo 3 de 3. Requiere `01-api-productoras.md` y `02-frontend-productoras.md` completados y confirmados con OK.

## 0. Rol y alcance

Eres un agente que ayuda al **Equipo C (Productoras)** a **verificar, empaquetar y entregar** su módulo según la sección 12 del taller. Aquí **no se programan funcionalidades nuevas**: solo se verifica, se corrige lo que falle (en los archivos del equipo) y se prepara la entrega.

---

## 1. Reglas de negocio a certificar

| # | Regla |
|---|---|
| R1 | Nombre obligatorio, máx. 100, inicial mayúscula |
| R2 | Fecha de fundación obligatoria, no futura, no anterior a 1900 |
| R3 | Sitio web obligatorio, máx. 300, URL válida |
| R4 | Logo opcional, jpg/png ≤ 4 MB |
| R5 | Caché `"productoras"` invalidado por POST/PUT/DELETE |
| R6 | PUT sin logo conserva el actual; con logo reemplaza y borra el viejo |
| R7 | DELETE borra el archivo del logo |
| R8 | Códigos HTTP: 201 (POST, con `Location`), 204 (PUT/DELETE), 400 (validación), 404 (id inexistente) |
| R9 | Sin reglas inventadas: no hay unicidad de nombre ni relación Productora–Película |

---

## 2. Protocolo de ciclo de respuesta (OBLIGATORIO en cada paso)

```
1. ANUNCIAR   → "Paso X: <nombre>. Objetivo: <una línea>."
2. EJECUTAR   → Haz solo lo que pide el paso. Nada extra.
3. VERIFICAR  → Corre el comando / la comprobación indicada.
4. REPORTAR   → Usa EXACTAMENTE el formato de abajo.
5. ESPERAR    → Pregunta: "¿Confirmas el Paso X? Responde OK o CORREGIR: <qué>."
6. DECIDIR    → OK → siguiente paso.  CORREGIR → repite el ciclo del mismo paso e indica qué cambiaste.
```

### Formato de reporte

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

1. **Nunca marques ✅ sin evidencia.** Lo que solo puede hacer el usuario (capturas de pantalla, probar en navegador, sustentación) se marca "pendiente del usuario" con instrucciones.
2. Una falla encontrada se corrige **solo** en los archivos del equipo; si el origen está en el contrato común, estado ❌ Bloqueado y avisar al usuario (que lo reporte al docente).
3. Si algo difiere de la guía, repórtalo en "Desviaciones".

---

## 3. Pasos

### Paso 1 — Verificación estática completa

```bash
# API (desde api_base)
dotnet build

# Frontend (desde frontend_base)
npx tsc --noEmit -p tsconfig.app.json
npx eslint src
npm run build
```

**Criterios:** las cuatro salidas con **0 errores**. Guarda la salida literal (será la "captura de la terminal" de la evidencia).

---

### Paso 2 — Pruebas de aceptación del API (Swagger)

Ejecuta con el API corriendo y reporta cada fila con ✅/❌ y evidencia (código HTTP y cuerpo de respuesta):

| # | Acción | Resultado esperado |
|---|---|---|
| 1 | POST con nombre `pixar` | 400: `La primera letra debe ser mayúscula` |
| 2 | POST con `fechaFundacion` 1850-01-01 | 400: `La fecha no puede ser anterior a 1900` |
| 3 | POST con `sitioWeb` `hola` | 400: `El campo SitioWeb debe ser una URL válida` |
| 4 | Crear Pixar, 03/02/1986, `https://www.pixar.com`, con logo | 201 + `Location`; aparece con logo y enlace |
| 5 | Editar sin cambiar el logo | 204; el logo sigue igual |
| 6 | Editar eligiendo otro logo | 204; el archivo anterior se borra |
| 7 | Borrar la productora | 204; desaparece y se borra el archivo |

---

### Paso 3 — Pruebas de límites de las reglas de negocio

Pruebas adicionales para confirmar que el **API** (no solo React) hace cumplir las reglas:

| # | Caso | Resultado esperado |
|---|---|---|
| L1 | `nombre` de 100 caracteres (con inicial mayúscula) | 201 |
| L2 | `nombre` de 101 caracteres | 400: `El campo Nombre debe tener 100 caracteres o menos` |
| L3 | `nombre` vacío o ausente | 400: `El campo Nombre es requerido` |
| L4 | `fechaFundacion` = 1900-01-01 | 201 (límite válido) |
| L5 | `fechaFundacion` = hoy | 201 (no es futura) |
| L6 | `fechaFundacion` = mañana | 400: `La fecha no puede ser del futuro` |
| L7 | `sitioWeb` de 301 caracteres | 400 por longitud |
| L8 | `sitioWeb` ausente | 400: `El campo SitioWeb es requerido` |
| L9 | `logo` = archivo `.pdf` | 400: `El archivo debe ser una imagen jpg o png` |
| L10 | `logo` jpg/png > 4 MB | 400 (el mensaje lo define `ImagenValidacion`; repórtalo literal) |
| L11 | Dos productoras con el mismo nombre | 201 ambas (**no** hay unicidad: R9) |
| L12 | PUT/DELETE/GET con id inexistente | 404 |
| L13 | Llamada directa al API (Swagger/curl) con datos inválidos, sin pasar por React | 400 (prueba que la validación no depende del frontend) |

> Cualquier ❌ aquí se corrige en `ProductoraCreacionDTO.cs` o `ProductorasController.cs` (archivos del equipo) y se repite el ciclo.

---

### Paso 4 — Pruebas de pantalla (UI) y del sistema completo

Lo que el agente no pueda ejecutar queda como "pendiente del usuario".

**Pantallas del módulo:** índice (logo, nombre, fecha, enlace, Editar/Borrar), crear, editar, borrar con confirmación, errores visibles, paginación (12 registros, 5 por página = 3 páginas).

**Del recorrido de sistema completo (sección 11 del taller) que tocan a Productoras:**

| # | Recorrido | Resultado esperado |
|---|---|---|
| 4 | Menú → Productoras: crear "Pixar" con logo | Aparece con su logo |
| 12 | Cerrar y volver a ejecutar el API | Los datos siguen ahí (SQL Server) |

**Nota de integración:** al copiar los módulos de los demás equipos sobre el proyecto base, ejecutar `dotnet build` y revisar posibles `ambiguous route` (cada ruta tiene un único dueño; las rutas de Productoras son `api/productoras`). El orden sugerido de integración es A, B y C primero (no dependen de nadie), luego D y E. Copiar **archivo por archivo o con Combinar (Merge)**, nunca reemplazar una carpeta completa.

---

### Paso 5 — Auditoría de descuentos (antes de empaquetar)

| Descuento | Comprobación | Resultado |
|---|---|---|
| Cada `any` o `@ts-ignore`: −3 pts | `grep -rnE "\bany\b\|@ts-ignore" src/features/productoras` | debe ser "limpio" |
| Modificar archivos del contrato común: hasta −10 pts | Comparar contra el proyecto base original (`git status`/`git diff` o comparador de carpetas): solo deben aparecer los **archivos del Equipo C** | ninguna diferencia fuera de la lista |
| Código idéntico a otro equipo: nota 1.0 | Que el trabajo sea propio; no copiar de otros equipos | — |
| Entrega tardía: −5 pts cada 5 min | Verificar la fecha/hora límite con el usuario | — |

**Lista permitida de archivos del Equipo C:**

```
DTOs/ProductoraCreacionDTO.cs
Utilidades/ProductorasProfile.cs
Controllers/ProductorasController.cs
src/features/productoras/   (carpeta completa)
```

---

### Paso 6 — Empaquetar `Equipo-C-Productoras.zip`

El zip contiene **SOLO** los archivos de arriba, con las mismas rutas del proyecto, y **sin** `node_modules`, `bin`, `obj` ni `Migrations`.

Estructura interna del zip:

```
Equipo-C-Productoras.zip
├── DTOs/ProductoraCreacionDTO.cs
├── Utilidades/ProductorasProfile.cs
├── Controllers/ProductorasController.cs
└── src/features/productoras/...
```

Ejemplo (Windows PowerShell, desde una carpeta de preparación `entrega/` con las rutas ya armadas):

```powershell
Compress-Archive -Path .\entrega\* -DestinationPath .\Equipo-C-Productoras.zip -Force
```

Ejemplo (Linux/macOS):

```bash
cd entrega && zip -r ../Equipo-C-Productoras.zip . -x "*/node_modules/*" "*/bin/*" "*/obj/*" "*/Migrations/*"
```

**Criterios:** listar el contenido del zip (`unzip -l` o equivalente) y confirmar que coincide con la lista permitida, sin archivos extra.

---

### Paso 7 — Documento de evidencias (PDF o Word)

Preparar con el usuario (el agente redacta la estructura y los textos; **las capturas las toma el usuario**):

1. **Primera página:** nombre del equipo (**C**), módulo (**Productoras**), nombres de los integrantes, fecha de inicio y fecha de entrega.
2. **Tres capturas de pantalla** del módulo funcionando: lista (`IndiceProductoras`), formulario de creación, formulario de edición.
3. **Una captura de la terminal** con `dotnet build`, `tsc`, `eslint` y `npm run build` sin errores (salida del Paso 1).
4. (Recomendado) tabla de pruebas de aceptación de los Pasos 2 y 3 con ✅.

Estado: "pendiente del usuario" hasta que confirme que adjuntó las capturas.

---

### Paso 8 — Preparar la sustentación individual (10 puntos)

Cada integrante debe poder explicar una parte del código. Genera una **guía de repaso** (resumen de 5–8 líneas por tema, en lenguaje simple):

- Por qué el DTO usa `IFormFile?` y por qué `[FromForm]` en vez de `[FromBody]`.
- Por qué el mapeo ignora `Logo` y quién asigna la URL (el almacenador).
- Cómo el PUT conserva o reemplaza el logo (y borra el archivo viejo).
- Qué hace `OutputCache` con la etiqueta `"productoras"` y por qué POST/PUT/DELETE llaman `EvictByTagAsync`.
- Por qué las validaciones están en el API y en React (el API no confía en el frontend).
- Cómo se arma el `FormData` y por qué `logo` solo se agrega si es un `File`.
- Cómo se leen `cantidad-total-registros` y la paginación.

---

## 4. Calificación de referencia (autoevaluación)

Nota = 1.0 + 4.0 × puntos ÷ 100.

| Criterio | Pts | Estado según evidencia |
|---|---|---|
| API: contrato y validaciones | 20 | (Pasos 2–3) |
| API: funcionamiento | 20 | (Pasos 2–3) |
| Frontend: pantallas | 20 | (Paso 4) |
| Frontend: calidad | 10 | (Paso 5) |
| Verificación | 10 | (Paso 1) |
| Integración | 10 | (Paso 4) |
| Sustentación individual | 10 | (Paso 8) |

---

## 5. Cierre final

Entrega un **reporte final** con: estado de cada paso (✅/⚠️/❌/pendiente del usuario), lista del contenido del zip, resultado de la auditoría de descuentos, tabla de pruebas y reglas R1–R9 certificadas. Termina con:

> "Entrega del Equipo C lista para revisión. ¿Confirmas con OK, o hay algo que corregir (CORREGIR: <qué>)?"
