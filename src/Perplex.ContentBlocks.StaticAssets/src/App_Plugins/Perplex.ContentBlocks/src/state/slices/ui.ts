import {createSlice} from "@reduxjs/toolkit";


const {actions: authActions, reducer: uiReducer} = createSlice({
    name: 'ui',
    initialState: {
        displayAddBlockModal: false,
    },
    reducers: {
        toggleAddBlockModal: (state) => {
            state.displayAddBlockModal = !state.displayAddBlockModal;
        },
        setAddBlockModal: (state, action) => {
            state.displayAddBlockModal = action.payload;
        }
    }
})

const {toggleAddBlockModal, setAddBlockModal} = authActions

export {uiReducer, setAddBlockModal, toggleAddBlockModal}
