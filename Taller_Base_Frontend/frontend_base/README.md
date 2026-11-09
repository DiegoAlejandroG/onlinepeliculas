# Frontend Cartelera de Películas (base del taller final)

Estado: **Géneros** ya está conectado al API. Actores, Cines, Productoras, Películas y la página principal
todavía usan datos simulados: cada equipo conecta el suyo.

## Puesta en marcha
1. `npm install`
2. Revisa `.env`: `VITE_API_URL=https://localhost:7256/api` (el puerto debe coincidir con el del API).
3. `npm run dev` → http://localhost:5173
4. Antes de entregar: `npx tsc --noEmit -p tsconfig.app.json`, `npx eslint src` y `npm run build`.
