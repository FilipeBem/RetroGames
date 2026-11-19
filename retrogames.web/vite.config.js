import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // Front-end e API na mesma origem para o navegador: dispensa CORS e permite cookie SameSite=Strict.
    proxy: {
      '/api': 'http://localhost:5080',
    },
  },
})
