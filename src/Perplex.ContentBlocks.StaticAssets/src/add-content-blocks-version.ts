import { promises as fs } from 'fs';
import { resolve, dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

function addContentBlocksVersion() {
    return {
        name: 'add-content-blocks-version',
        apply: 'build',
        closeBundle: async () => {
            try {
                // 1. Read Directory.Build.props 4 directories up
                const propsPath = resolve(__dirname, '../../Directory.Build.props');
                const propsContent = await fs.readFile(propsPath, 'utf-8');

                // 2. Extract <Version> with regex
                const versionMatch = propsContent.match(/<Version>(.*?)<\/Version>/);
                if (!versionMatch) {
                    throw new Error('Version tag not found in Directory.Build.props');
                }
                const version = versionMatch[1];

                // 3. Read and update umbraco-package.json
                const distFilePath = resolve(
                    __dirname,
                    '../wwwroot/App_Plugins/Perplex.ContentBlocks/umbraco-package.json',
                );
                let content = await fs.readFile(distFilePath, 'utf-8');
                content = content.replace('perplex.content-blocks.js', `perplex.content-blocks.js?v=${version}`);
                await fs.writeFile(distFilePath, content, 'utf-8');
            } catch (err) {
                console.error('Failed to update umbraco-package.json:', err);
            }
        },
    };
}

export default addContentBlocksVersion;
