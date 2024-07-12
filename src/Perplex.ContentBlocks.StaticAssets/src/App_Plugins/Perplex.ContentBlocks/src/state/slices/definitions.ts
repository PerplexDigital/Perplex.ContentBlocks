import {createSlice} from "@reduxjs/toolkit";
import {PerplexBlockDefinition} from "../../types.ts";

export interface DefinitionsDictionary {
    [key: string]: PerplexBlockDefinition
}

const {actions: authActions, reducer: definitionsReducer} = createSlice({
    name: 'definitions',
    initialState: {
        value: {}
    },
    reducers: {
        setDefinitions: (state, action) => {
            // Convert array to DefinitionsDictionary
            state.value = action.payload.reduce((acc: DefinitionsDictionary, curr: PerplexBlockDefinition) => {
                acc[curr.id] = {
                    ...curr,
                }

                return acc;
            }, {})
        }
    }
})

const {setDefinitions} = authActions

export {definitionsReducer, setDefinitions}
