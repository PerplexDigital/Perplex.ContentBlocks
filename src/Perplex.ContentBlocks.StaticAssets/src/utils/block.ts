import { PCBCategory, PCBCategoryWithDefinitions, PerplexContentBlocksBlock } from '../types';

export function propertyAliasPrefix(block: PerplexContentBlocksBlock): string {
    return block.id + '_';
}

export function getCategoriesForDefinition(definitionId: string, items: PCBCategoryWithDefinitions[]): PCBCategory[] {
    return items.filter(x => x.definitions[definitionId]).map(x => x.category);
}
