import {configureStore} from '@reduxjs/toolkit'
import {definitionsReducer} from "./slices/definitions.ts";
import {uiReducer} from "./slices/ui.ts";

const store = configureStore({
    reducer: {
        definitions: definitionsReducer,
        ui: uiReducer,
    }
})

export { store }