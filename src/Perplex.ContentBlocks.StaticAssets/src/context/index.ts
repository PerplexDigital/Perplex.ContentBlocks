import { createContext } from '@lit/context';
import type { PcbEditorContext } from './pcb-editor-context.ts';

/** Rich editor context carrying definitions, caches, and shared state. */
export const pcbEditorContext = createContext<PcbEditorContext>('pcbEditorContext');
