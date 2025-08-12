import { PerplexBlockDefinition, PerplexContentBlocksBlock, Section } from '../types.ts';
export const ON_BLOCK_HEAD_CLICK = 'ON_BLOCK_HEAD_CLICK';
export const ON_BLOCK_SELECTED = 'ON_BLOCK_SELECTED';
export const ON_BLOCK_SAVED = 'ON_BLOCK_SAVED';
export const ON_BLOCK_UPDATED = 'ON_BLOCK_CHANGED';
export const ON_BLOCK_REMOVE = 'ON_BLOCK_REMOVE';
export const ON_BLOCK_TOGGLE = 'ON_BLOCK_TOGGLE';
export const ON_BLOCK_LAYOUT_CHANGE = 'ON_BLOCK_LAYOUT_CHANGE';

type BlockCreation = {
    block: PerplexContentBlocksBlock;
    section: Section;
};

export const BlockSavedEvent = (blockCreation: BlockCreation) =>
    new CustomEvent(ON_BLOCK_SAVED, {
        detail: {
            block: blockCreation.block,
            section: blockCreation.section,
        },
        bubbles: true,
        cancelable: true,
        composed: true,
    });

export const BlockToggleEvent = (id: string) =>
    new CustomEvent(ON_BLOCK_TOGGLE, {
        detail: {
            id,
        },
        bubbles: true,
        cancelable: true,
        composed: true,
    });

export const BlockUpdatedEvent = (block: PerplexContentBlocksBlock, definition: PerplexBlockDefinition) =>
    new CustomEvent(ON_BLOCK_UPDATED, {
        detail: {
            block,
            definition,
        },
        bubbles: true,
        cancelable: true,
        composed: true,
    });

export const BLockLayoutChangeEvent = (selectedLayout: { id: string; name: string }) =>
    new CustomEvent(ON_BLOCK_LAYOUT_CHANGE, {
        detail: {
            selectedLayout,
        },
        bubbles: true,
        cancelable: true,
        composed: true,
    });
