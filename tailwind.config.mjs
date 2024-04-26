/** @type {import('tailwindcss').Config} */
export default {
  content: ['./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}'],
  theme: {
    extend: {
      gridTemplateColumns: { 40: 'repeat(40, minmax(0, 1fr))' },
      gridTemplateRows: { 40: 'repeat(40, minmax(0, 1fr))' }
    }
  }
}
