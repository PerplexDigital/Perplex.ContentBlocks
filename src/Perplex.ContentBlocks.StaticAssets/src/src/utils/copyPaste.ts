import { PerplexContentBlocksBlock } from '../types.ts';

export const differentiateBlocks = (blocks: PerplexContentBlocksBlock[]) => {
    return blocks.map((block: PerplexContentBlocksBlock) => {
        return {
            ...block,
            id: crypto.randomUUID(),
            content: {
                ...block.content,
                key: crypto.randomUUID(),
            },
        };
    });
};
