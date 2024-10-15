import { createSlice } from '@reduxjs/toolkit';
import { Section } from '../../types.ts';

type UiState = {
    addBlock: {
        display: boolean;
        section: Section;
    };
};

const { actions: authActions, reducer: uiReducer } = createSlice({
    name: 'ui',
    initialState: {
        addBlock: {
            display: false,
            section: 'content',
        },
    } as UiState,
    reducers: {
        toggleAddBlockModal: (state) => {
            state.addBlock.display = !state.addBlock.display;
        },
        setAddBlockModal: (state, action) => {
            state.addBlock.display = action.payload.display;
            state.addBlock.section = action.payload.section;
        },
        resetAddBlockModal: (state) => {
            state.addBlock.display = false;
            state.addBlock.section = 'content';
        },
    },
});

const { toggleAddBlockModal, setAddBlockModal, resetAddBlockModal } = authActions;

export { uiReducer, setAddBlockModal, toggleAddBlockModal, resetAddBlockModal };
