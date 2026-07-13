import { defineConfig } from 'vite';
import { resolve } from 'path';
import checker from 'vite-plugin-checker';

export default defineConfig({
    root: resolve(__dirname, './'),
    publicDir: 'public',
    plugins: [
        checker({
            stylelint: {
                lintCommand: 'stylelint "./src/scss/**/*.scss"',
            },
            eslint: {
                lintCommand: 'eslint "./src/js/**/*.js"',
            },
            overlay: {
                initialIsOpen: false,
                position: 'br',
            },
        }),
    ],
    build: {
        outDir: resolve(__dirname, '../SvConcatWeb/wwwroot/dist'),
        emptyOutDir: true,
        rollupOptions: {
            input: {
                main: resolve(__dirname, 'src/js/main.js'),
                styles: resolve(__dirname, 'src/scss/main.scss'),
            },
            output: {
                // Entry + CSS keep stable names (referenced from Master.cshtml with
                // asp-append-version for cache-busting). Lazy-loaded component chunks
                // are content-hashed so an updated component can never be served stale
                // from a browser/proxy cache; main.js imports them by their hashed name.
                entryFileNames: '[name].js',
                chunkFileNames: '[name]-[hash].js',
                assetFileNames: '[name].[ext]',
                format: 'es',
            },
        },
        minify: false,
        sourcemap: true,
    },
});