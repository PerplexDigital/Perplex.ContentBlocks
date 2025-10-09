import { configureStore } from '@reduxjs/toolkit';
import { definitionsReducer } from './slices/definitions.ts';
import { uiReducer } from './slices/ui.ts';
import { copyPasteReducer } from './slices/copyPaste.ts';
import { persistReducer, persistStore } from 'redux-persist';
import storageSession from 'redux-persist/lib/storage/session';

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
        ui: uiReducer,
    },
});

const persistor = persistStore(store);

export { store, persistor };
