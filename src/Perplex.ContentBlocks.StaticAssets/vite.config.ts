import { defineConfig } from 'vite';
import { viteStaticCopy } from 'vite-plugin-static-copy';
import addContentBlocksVersion from './add-content-blocks-version.ts';

export default defineConfig({
    build: {
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
    plugins: [
        viteStaticCopy({
            targets: [
                {
                    src: 'umbraco-package.json',
                    dest: '.',
                },
            ],
        }),
        addContentBlocksVersion(),
    ],
});
