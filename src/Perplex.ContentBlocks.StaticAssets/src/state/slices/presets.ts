import { createSlice } from '@reduxjs/toolkit';
import { Preset } from '../../types.ts';

export type PresetState = {
    value: Preset;
};

const { actions: authActions, reducer: presetsReducer } = createSlice({
    name: 'presets',
    initialState: {
        value: {},
    } as PresetState,
    reducers: {
        setPresets: (state, action) => {
            state.value = action.payload;
        },
    },
});

const { setPresets } = authActions;

export { presetsReducer, setPresets };
