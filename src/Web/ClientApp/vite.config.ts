import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT!),
    proxy: {
      '/api': {
        target: 'http://localhost:5073',
        secure: false,
        changeOrigin: true,
      },
      '/openapi': {
        target: 'http://localhost:5073',
        changeOrigin: true,
      },
      '/scalar': {
        target: 'http://localhost:5073',
        changeOrigin: true,
      },

    },
  },
  build: {
    outDir: 'build',
  },
});

