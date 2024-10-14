import {PerplexContentBlocksBlock} from "../types.ts";
export const ON_BLOCK_HEAD_CLICK = 'ON_BLOCK_HEAD_CLICK'
export const ON_BLOCK_SELECTED = 'ON_BLOCK_SELECTED'
export const ON_BLOCK_SAVED = 'ON_BLOCK_SAVED';
export const ON_BLOCK_REMOVE = 'ON_BLOCK_REMOVE';
export const ON_BLOCK_TOGGLE = 'ON_BLOCK_TOGGLE';

export const BlockSavedEvent = (block: PerplexContentBlocksBlock) => new CustomEvent(ON_BLOCK_SAVED, {
    detail: {
        block: block,
    },
    bubbles: true,
    cancelable: true,
    composed: true,
});

export const BlockToggleEvent = (id: string) => new CustomEvent(ON_BLOCK_TOGGLE, {
    detail: {
        id,
    },
    bubbles: true,
    cancelable: true,
    composed: true,
});