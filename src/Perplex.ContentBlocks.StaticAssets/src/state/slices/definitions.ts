import { createSlice } from '@reduxjs/toolkit';
import { PCBCategoryWithDefinitions, PerplexBlockDefinition } from '../../types.ts';

export interface DefinitionsDictionary {
    [key: string]: PerplexBlockDefinition;
}

export type DefinitionsState = {
    value: PCBCategoryWithDefinitions[];
};

const { actions: authActions, reducer: definitionsReducer } = createSlice({
    name: 'definitions',
    initialState: {
        value: [],
    } as DefinitionsState,
    reducers: {
        setDefinitions: (state, action) => {
            state.value = action.payload;
        },
    },
});

const { setDefinitions } = authActions;

export { definitionsReducer, setDefinitions };
