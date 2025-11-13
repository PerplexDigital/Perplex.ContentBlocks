import { PerplexContentBlocksBlock } from '../types';

export function propertyAliasPrefix(block: PerplexContentBlocksBlock): string {
    return block.id + '_';
}
