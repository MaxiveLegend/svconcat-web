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
                entryFileNames: '[name].js',
                chunkFileNames: '[name].js',
                assetFileNames: '[name].[ext]',
                format: 'es',
            },
        },
        minify: false,
        sourcemap: true,
    },
});