import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { PerplexContentBlocksBlock } from '../../types.ts';
import { differentiateBlocks } from '../../utils/copyPaste.ts';

export type CopiedData = {
    header: PerplexContentBlocksBlock | null;
    blocks: PerplexContentBlocksBlock[];
};

export type CopyPasteState = {
    copied: CopiedData | null;
};

const { actions, reducer: copyPasteReducer } = createSlice({
    name: 'copyPaste',
    initialState: {
        copied: null,
    } as CopyPasteState,
    reducers: {
        setCopiedValue: (state, action: PayloadAction<CopiedData>) => {
            state.copied = {
                header: action.payload.header ? differentiateBlocks([action.payload.header])[0] : null,
                blocks: differentiateBlocks(action.payload.blocks),
            };
        },
    },
});

const { setCopiedValue } = actions;

export { copyPasteReducer, setCopiedValue };
