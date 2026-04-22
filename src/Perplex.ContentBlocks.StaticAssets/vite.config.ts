import { defineConfig } from 'vite';

export default defineConfig({
    build: {
        target: ['chrome112', 'edge112', 'firefox117', 'safari16.5'],
        lib: {
            entry: 'src/index.ts',
            formats: ['es'],
        },
        outDir: './wwwroot/App_Plugins/Perplex.ContentBlocks/',
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    },
    define: {
        'process.env': {},
    },
});
