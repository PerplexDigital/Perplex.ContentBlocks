import { defineConfig } from 'vite';
import path from 'path';
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
            input: {
                'perplex.content-blocks': 'src/index.ts',
                AddBlockModal: 'src/components/modals/addBlock/pcb-add-block-modal.ts',
            },
            output: {
                entryFileNames: '[name].js',
            },
        },
    },
    define: {
        'process.env': {},
    },
    plugins: [addContentBlocksVersion()],
});
