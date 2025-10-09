import { createSlice } from '@reduxjs/toolkit';
import { PerplexContentBlocksBlock } from '../../types.ts';
import { differentiateBlocks } from '../../utils/copyPaste.ts';

export type CopyPasteState = {
    copied: PerplexContentBlocksBlock[] | null;
};

const { actions, reducer: copyPasteReducer } = createSlice({
    name: 'copyPaste',
    initialState: {
        copied: null,
    } as CopyPasteState,
    reducers: {
        setCopiedValue: (state, action) => {
            state.copied = differentiateBlocks(action.payload);
        },
    },
});

const { setCopiedValue } = actions;

export { copyPasteReducer, setCopiedValue };
