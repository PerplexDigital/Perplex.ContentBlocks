import { configureStore } from '@reduxjs/toolkit';
import { definitionsReducer, DefinitionsState } from './slices/definitions.ts';
import { presetsReducer, PresetState } from './slices/presets.ts';
import { uiReducer, UiState } from './slices/ui.ts';
import { copyPasteReducer, CopyPasteState } from './slices/copyPaste.ts';
import { persistReducer, persistStore } from 'redux-persist';
import storageSession from 'redux-persist/lib/storage/session';

export type AppState = {
    definitions: DefinitionsState;
    copyPaste: CopyPasteState;
    presets: PresetState;
    ui: UiState;
};

const persistedReducer = persistReducer(
    {
        key: 'pcb-copy-paste',
        storage: storageSession,
    },
    copyPasteReducer,
);

const store = configureStore({
    reducer: {
        copyPaste: persistedReducer,
        definitions: definitionsReducer,
        presets: presetsReducer,
        ui: uiReducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({
            serializableCheck: false,
        }),
});

const persistor = persistStore(store);

export { store, persistor };
