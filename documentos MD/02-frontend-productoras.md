# 02 · Frontend React de Productoras (Equipo C) — Guía para el agente

> Archivo 2 de 3. Requiere haber completado y confirmado `01-api-productoras.md` (API corriendo con `/api/productoras`).

## 0. Rol y alcance

Eres un agente de programación que ayuda al **Equipo C (Productoras)**. Este archivo cubre **solo el frontend** (React + TypeScript + Vite + axios + Yup). Hoy Productoras usa **datos de ejemplo**; tu trabajo es conectarlo al API real, replicando el patrón de **Géneros**.

### Carpeta que SÍ puedes crear/editar

```
frontend_base/src/features/productoras/   (toda la carpeta)
```

Estructura sugerida (respeta la que ya exista):

```
componentes/  IndiceProductoras.tsx, CrearProductora.tsx, EditarProductora.tsx, FormularioProductora.tsx
modelos/      Productora.model.ts
utilidades/   convertirProductoraAFormData.ts
```

### NO modificar

Todo lo que esté fuera de `src/features/productoras/`: componentes compartidos (`clienteAPI`, `useEntidades`, `IndiceEntidades`, `Paginacion`, `confirmar`, `extraerErrores`, `MostrarErrores`), `Validaciones.ts`, router, `.env`, y el feature de Géneros. Si necesitas cambiar algo ahí (p. ej. una ruta), **detente y pregunta al usuario**.

---

## 1. Reglas de negocio aplicables al frontend

| # | Regla | Cómo se refleja en React |
|---|---|---|
| R1 | Nombre obligatorio, inicial mayúscula (el API limita a 100) | Yup + mostrar error del API |
| R2 | Fecha de fundación obligatoria, no futura, no anterior a 1900 | Yup (reutilizar `fechaNoAnteriorA1900` del taller anterior) + `input type="date"` |
| R3 | Sitio web obligatorio, formato URL válido (el API limita a 300) | Yup `url()` + mostrar error del API |
| R4 | Logo opcional, jpg/png ≤ 4 MB | Validar tipo/tamaño al elegir el archivo (opcional) + error del API |
| R5 | La lista debe reflejar lo recién creado/editado | Recargar la lista tras mutaciones (el API invalida el caché) |
| R6 | Editar sin elegir logo conserva el actual | **No** enviar `logo` en el FormData si no es un `File` |
| R7 | Borrar pide confirmación | `confirmar()` antes de `DELETE` |

**Recordatorios del documento de reglas de negocio:**

- Las validaciones del frontend **no son garantía**: el API es la autoridad. Aun así, debes **mostrar los errores 400 del API** con `extraerErrores` + `MostrarErrores`.
- No agregues reglas inexistentes (unicidad de nombre, relación con películas).

**Calidad exigida (se califica):** sin `any`, sin `@ts-ignore`; patrón de Géneros respetado; errores del API visibles; confirmación al borrar.

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

1. **No marques ✅ lo que no verificaste.** Si no puedes ejecutar el navegador, di "no verificado en UI" y lista qué debe probar el usuario.
2. Si el código existente difiere de lo que supone esta guía (nombres de props, firmas de hooks), **adáptate a lo real** y repórtalo en "Desviaciones".
3. Prohibido `any` y `@ts-ignore` (cada uno resta 3 puntos).
4. Un cambio por paso. Si un paso exige tocar archivos fuera de `src/features/productoras/`, estado ❌ Bloqueado y pregunta.

---

## 3. Pasos

### Paso 0 — Reconocimiento (solo lectura)

**Objetivo:** copiar el patrón correcto.

Lee y resume:

1. `src/features/generos/` completo: índice, crear, editar, formulario, modelo → es **el molde**.
2. `src/features/productoras/` actual: qué usa (datos de ejemplo), qué props recibe `FormularioProductora`, qué esquema Yup tiene (incluida `fechaNoAnteriorA1900`).
3. Utilidades compartidas: firmas de `clienteAPI`, `useEntidades`, `IndiceEntidades`, `Paginacion`, `confirmar`, `extraerErrores`, `MostrarErrores`.
4. Rutas existentes de Productoras en el router (no modificarlas).
5. `.env`: `VITE_API_URL=https://localhost:7256/api` (puerto https de `launchSettings.json`).

**Criterios:** resumen de patrón (cómo Géneros lista, crea, edita, borra y muestra errores). **Cero archivos modificados.**

---

### Paso 1 — Preparar el frontend

```bash
npm install
npm run dev
```

**Criterios:** Vite arranca (puerto `http://localhost:5173`); la pantalla de Géneros lista/crea/edita/borra contra el API (comprueba CORS); el API del archivo 01 está corriendo.

---

### Paso 2 — Modelo `modelos/Productora.model.ts`

```ts
export interface Productora {
  id: number;
  nombre: string;
  fechaFundacion: string;   // 'aaaa-mm-dd'
  sitioWeb: string;
  logo?: string;            // URL devuelta por el API
}

export interface ProductoraCreacion {
  nombre: string;
  fechaFundacion: string;
  sitioWeb: string;
  logo?: File | string;     // File = nuevo; string = URL actual (no se reenvía)
}
```

**Criterios:** nombres en camelCase iguales a las propiedades JSON del API (`fechaFundacion`, `sitioWeb`, `logo`); sin `any`.

---

### Paso 3 — `utilidades/convertirProductoraAFormData.ts`

```ts
import type { ProductoraCreacion } from '../modelos/Productora.model';

export default function convertirProductoraAFormData(p: ProductoraCreacion): FormData {
  const formData = new FormData();
  formData.append('nombre', p.nombre);
  formData.append('fechaFundacion', p.fechaFundacion); // input type="date" ya da aaaa-mm-dd
  formData.append('sitioWeb', p.sitioWeb);

  // Si logo es texto, es la URL actual (edición sin cambiar imagen): no se envía (R6).
  if (p.logo && p.logo instanceof File) {
    formData.append('logo', p.logo);
  }
  return formData;
}
```

**Criterios:** los nombres de campo coinciden con el DTO del API (no distinguen mayúsculas, pero `imagen` ≠ `logo`); la fecha va tal cual `aaaa-mm-dd`.

---

### Paso 4 — `componentes/FormularioProductora.tsx`

**Objetivo:** formulario reutilizable para crear/editar, como `FormularioGenero`.

Requisitos:

1. Mantener las reglas Yup del taller anterior: nombre requerido con inicial mayúscula; fecha requerida, no futura y no anterior a 1900 (`fechaNoAnteriorA1900`); sitio web requerido con `url()`.
2. **Agregar la prop `errores: string[]`** y renderizar `<MostrarErrores errores={errores} />`.
3. Campo logo: `input type="file"` (accept `image/jpeg,image/png`); mostrar el logo actual (URL) en edición y una vista previa si se elige uno nuevo; guardar el `File` en el estado.
4. Tipar todo con `ProductoraCreacion`; sin `any`.

**Criterios:** el formulario valida en cliente, muestra errores del API, y en edición muestra el logo actual.

---

### Paso 5 — `componentes/CrearProductora.tsx`

1. Estado `errores: string[]`.
2. `onSubmit`: `convertirProductoraAFormData(valores)` → `clienteAPI.post('/productoras', formData, { headers: { 'Content-Type': 'multipart/form-data' } })`.
3. Éxito → navegar al índice de productoras.
4. `catch` → `extraerErrores(error)` → `setErrores(...)`.

**Criterios:** crear "Pixar / 1986-02-03 / https://www.pixar.com / logo" aparece en el índice; un nombre en minúscula muestra el error (cliente o API).

---

### Paso 6 — `componentes/EditarProductora.tsx`

1. Cargar con `GET /productoras/{id}` (ya sea con el hook del patrón de Géneros o con `useEffect` + `clienteAPI.get`); mostrar estado de carga.
2. Pasar al formulario el modelo con `logo` = URL actual.
3. `onSubmit`: `clienteAPI.put('/productoras/{id}', formData, { headers: { 'Content-Type': 'multipart/form-data' } })`.
4. Sin logo nuevo → el FormData **no** lleva `logo` (el API conserva el actual — R6).
5. Éxito → navegar al índice; error → `extraerErrores`.

**Criterios:** editar sin cambiar logo mantiene el logo; elegir otro logo lo reemplaza.

---

### Paso 7 — `componentes/IndiceProductoras.tsx`

Usar `useEntidades` e `IndiceEntidades` + `Paginacion` **igual que Géneros**, contra `/productoras`.

Columnas: **logo (miniatura)**, nombre, fecha de fundación, **sitio web como enlace** (`target="_blank"` con `rel="noopener noreferrer"`), botones **Editar** y **Borrar**.

- Borrar: `confirmar(...)` → `clienteAPI.delete('/productoras/{id}')` → recargar lista (R7).
- La paginación depende de la cabecera `cantidad-total-registros` (si muestra `NaN`, la cabecera no llegó).
- Fila sin logo: mostrar un marcador (no romper la tabla).

**Criterios:** la lista viene del API real (no de datos de ejemplo); se elimina cualquier uso de datos simulados dentro de la carpeta de productoras.

---

### Paso 8 — Verificación estática y prueba de pantallas

Desde el frontend:

```bash
npx tsc --noEmit -p tsconfig.app.json
npx eslint src
npm run build
```

Todos deben terminar **sin errores**. Además:

```bash
# Auditoría de descuentos (cada any o @ts-ignore resta 3 puntos)
grep -rnE "\bany\b|@ts-ignore" src/features/productoras || echo "limpio"
```

Pruebas manuales en la UI (marca ✅/❌ con lo que realmente se probó):

| # | Acción | Resultado esperado |
|---|---|---|
| 1 | Crear "Pixar", 03/02/1986, `https://www.pixar.com`, con logo | Aparece en el índice con logo y enlace |
| 2 | Crear con nombre `pixar` | Se muestra el error de mayúscula |
| 3 | Crear con sitio web `hola` | Se muestra el error de URL |
| 4 | Crear con fecha anterior a 1900 / futura | Se muestra el error de fecha |
| 5 | Editar sin cambiar logo | Cambios guardados; el logo sigue |
| 6 | Editar eligiendo otro logo | El logo cambia |
| 7 | Borrar | Pide confirmación; desaparece de la lista |
| 8 | Crear 12 productoras, 5 por página | 3 páginas, total 12 |

**Errores frecuentes:** `Failed to resolve import` → falta `npm install` o reemplazaste carpetas completas; CORS → el origen debe ser `http://localhost:5173`; `400 The Logo field is required` → se envió JSON en vez de `FormData`; el archivo llega `null` → el nombre del campo del FormData no coincide con el del DTO.

---

## 4. Cierre del archivo 02

Entrega un **resumen final**: archivos creados, resultado de `tsc`/`eslint`/`build`, resultado del `grep` de `any`/`@ts-ignore`, tabla de pruebas manuales y reglas R1–R7 cubiertas. Termina con:

> "Archivo 02 completado. ¿Confirmas con OK para continuar con `03-verificacion-y-entrega.md`?"
