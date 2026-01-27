import { createSlice } from '@reduxjs/toolkit';
import { Section } from '../../types.ts';

export type UiState = {
    addBlock: {
        display: boolean;
        section: Section;
        insertAtIndex?: number;
    };
    isDraggingBlock: boolean;
    isTouchDevice?: boolean;
};

const { actions: uiActions, reducer: uiReducer } = createSlice({
    name: 'ui',
    initialState: {
        addBlock: { display: false, section: Section.CONTENT },
        isDraggingBlock: false,
    } as UiState,
    reducers: {
        toggleAddBlockModal: state => {
            state.addBlock.display = !state.addBlock.display;
        },
        setAddBlockModal: (state, action) => {
            state.addBlock.display = action.payload.display;
            state.addBlock.section = action.payload.section;
            state.addBlock.insertAtIndex = action.payload.insertAtIndex ?? null;
        },
        resetAddBlockModal: state => {
            state.addBlock.display = false;
            state.addBlock.section = Section.CONTENT;
            state.addBlock.insertAtIndex = undefined;
        },
        setIsDraggingBlock: (state, action: { payload: boolean }) => {
            state.isDraggingBlock = action.payload;
        },
        setIsTouchDevice: (state, action: { payload: boolean }) => {
            state.isTouchDevice = action.payload;
        },
    },
});

export const { toggleAddBlockModal, setAddBlockModal, resetAddBlockModal, setIsDraggingBlock, setIsTouchDevice } =
    uiActions;
export { uiReducer };
