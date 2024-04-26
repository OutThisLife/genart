import react from '@astrojs/react'
import tailwind from '@astrojs/tailwind'
import { defineConfig } from 'astro/config'
import glsl from 'vite-plugin-glsl'

// https://astro.build/config
export default defineConfig({
  build: { target: 'esnext' },
  integrations: [tailwind(), react()],
  server: {
    headers: { 'Cache-Control': 'public, max-age=0, must-revalidate' }
  },
  vite: {
    plugins: [glsl()]
  }
})
