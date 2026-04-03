# Perplex.ContentBlocks Performance Rewrite Plan

## Problem Statement

The Perplex.ContentBlocks property editor exhibits sluggish performance in the Umbraco backoffice. Root causes identified through full codebase analysis:

1. **Redux global store + pwa-helpers connect** causes 60+ components to re-render on every state change
2. **Redundant HTTP requests** — each block independently fetches content types and data types
3. **Swiper library** creates 40+ carousel instances per page
4. **@lit-labs/motion** adds animation overhead to every block in repeat lists
5. **Sequential API calls** where parallel would suffice

---

## Architectural Changes

| Area | Current | New |
|---|---|---|
| State management | Redux + pwa-helpers connect (60+ re-renders per dispatch) | `@lit/context` + Lit ReactiveControllers |
| Dependencies removed | `@reduxjs/toolkit`, `pwa-helpers`, `redux-persist`, `swiper`, `@lit-labs/motion` | Only `mutative` + `@lit/context` remain |
| Layout switcher | Swiper (40+ instances) | Lightweight custom CSS/button-based switcher |
| Content type fetching | Each block fetches independently | Shared `ContentTypeCache` at editor level |
| Data type fetching | Each block fetches independently | Shared `DataTypeCache` at editor level |
| API calls | 3 sequential calls | `Promise.all()` (parallel) |
| Copy/paste storage | Redux slice + redux-persist to sessionStorage | Direct sessionStorage read/write |
| Animations | `@lit-labs/motion` animate on every block | CSS transitions only (expand/collapse) |
| Event listeners | Added/removed in `updated()` lifecycle | Added once in `connectedCallback`, removed in `disconnectedCallback` |

---

## File-by-File Plan

### 1. `package.json` — Remove Dependencies

**Remove:**
- `@reduxjs/toolkit` (Redux)
- `pwa-helpers` (Redux-Lit bridge)
- `redux-persist` (sessionStorage persistence)
- `swiper` (carousel library)
- `@lit-labs/motion` (animation)

**Keep:**
- `mutative` (immutable updates in dataset context)
- `@lit/context` (Lit context protocol)

### 2. Delete `state/` Directory (Entire)

Delete all files:
- `state/store.ts`
- `state/slices/definitions.ts`
- `state/slices/copyPaste.ts`
- `state/slices/presets.ts`
- `state/slices/ui.ts`

### 3. New `context/pcb-editor-context.ts` — Central Reactive Context

Replaces ALL Redux state. Single context provided by main editor, consumed by children via `@consume`.

**Data provided:**
- `editorId: string`
- `definitions: PCBCategoryWithDefinitions[]`
- `definitionsMap: Map<string, PerplexBlockDefinition>` (precomputed lookup)
- `headerCategories: string[]`
- `presets: Preset | null`
- `copiedValue: CopiedData | null` (backed by sessionStorage)
- `isDraggingBlock: boolean`
- `isTouchDevice: boolean`

**Shared caches provided:**
- `contentTypeCache: Map<string, Promise<UmbDocumentTypeDetailModel>>` — deduplicates content type requests
- `dataTypeCache: Map<string, Promise<UmbDataTypeDetailModel>>` — deduplicates data type requests

**Methods:**
- `setCopied(data: CopiedData)` — writes to sessionStorage + notifies consumers
- `getCopied(): CopiedData | null` — reads from sessionStorage
- `getContentType(key: string): Promise<UmbDocumentTypeDetailModel>` — returns cached or fetches
- `getDataType(key: string): Promise<UmbDataTypeDetailModel>` — returns cached or fetches

### 4. `context/index.ts` — Update

Replace the simple `editorContext` string key with the new `PcbEditorContext` object context.

### 5. `editor/perplex-content-blocks.ts` — Main Editor (Full Rewrite)

**Remove:**
- `connect(store)(UmbLitElement)` mixin
- All Redux imports, dispatches, and `stateChanged()` method
- `@lit-labs/motion` `animate()` directive usage

**Add:**
- Extend plain `UmbLitElement`
- Create and `@provide` the new `PcbEditorContext` instance
- Fetch definitions + categories + presets in `Promise.all()` (parallel)
- Store definitions/presets in the context
- Compute `isMandatory` for blocks here and pass as prop (instead of each block computing it via Redux)
- Pass `isDraggingBlock`, `hasCopiedValue` as props to children (not global state)

**Keep:**
- All event handling logic (add, remove, update, copy, paste, reorder)
- All CSS/styling
- Preview integration
- Debug mode

### 6. `components/block/pcb-block.ts` — Block Component (Full Rewrite)

**Remove:**
- `connect(store)` mixin and `stateChanged()`
- `@lit-labs/motion` `animate({ id: block.id })`
- Independent `UmbDocumentTypeDetailRepository` and `UmbDataTypeDetailRepository` instances
- Adding/removing drag listeners in `updated()` (move to `connectedCallback`/`disconnectedCallback`)

**Add:**
- `@consume` the editor context for shared caches
- Use `context.getContentType(key)` and `context.getDataType(key)` for deduplicated lookups
- Receive `isDraggingBlock` and `isMandatory` as `@property` from parent

**Keep:**
- Validation logic
- Lazy-loading body (only when expanded)
- All CSS/styling
- The `#getOrderedProperties()` logic for compositions

### 7. `components/block/blockHead/pcb-block-head.ts` — Block Head (Full Rewrite)

**Remove:**
- `connect(store)` mixin and `stateChanged()`
- Redux state subscriptions for `isTouchDevice` and `categoryWithDefinitions`

**Add:**
- Receive `isTouchDevice` and `categoryWithDefinitions` as `@property` from parent
- (Alternative: `@consume` context if needed, but props are simpler and more explicit)

**Keep:**
- All render logic, UFM rendering, icon lookup
- All CSS/styling (block-head.css)
- Copy, remove, visibility controls

### 8. `components/block/blockSpacer/pcb-block-spacer.ts` — Block Spacer (Full Rewrite)

**Remove:**
- `connect(store)` mixin and `stateChanged()`
- Redux subscription for `copiedValue`

**Add:**
- Receive `hasCopiedValue: boolean` as `@property` from parent
- Receive paste handler as callback prop

**Keep:**
- All CSS/styling (pcb-block-spacer.css)
- Add block and paste block functionality

### 9. `components/block/inlineLayoutSwitch/pcb-inline-layout-switch.ts` — Layout Switcher (Full Rewrite)

**Remove:**
- Swiper entirely (2 swiper-container instances per block)
- `initSwiper()` calls

**Replace with:**
- Simple button/dropdown-based layout picker
- Previous/next buttons wrapping the layout name
- Hover preview using pure CSS (show preview image on hover)
- Same visual footprint, vastly less DOM and JS overhead

**Keep:**
- CSS styling structure (pcb-inline-layout-switch.css will be updated)
- `PcbBlockLayoutChangeEvent` dispatch on layout change
- Lazy preview image loading

### 10. `components/block/blockDefinition/pcb-block-definition.ts` — Block Definition Card (Rewrite)

**Remove:**
- Swiper carousel for layout navigation
- `initSwiper()` calls

**Replace with:**
- Simple prev/next buttons or dot indicators for multiple layouts
- Pure CSS transitions between layout slides
- Keep the same visual card structure (image + name + description)

**Keep:**
- CSS styling (pcb-block-definition.css)
- `ON_BLOCK_SELECTED` event dispatch
- Layout selection state

### 11. `components/dragAndDrop/` — Minor Cleanup

**pcb-drag-and-drop.ts:**
- Keep as-is (already uses RAF throttling efficiently)

**pcb-drag-item.ts:**
- Move event listener add/remove from `updated()` to `connectedCallback`/`disconnectedCallback`
- Use a simple `if (this.canDrag)` check in handlers instead

### 12. `components/modals/addBlock/pcb-add-block-modal.ts` — Minor Cleanup

- No Redux dependencies to remove (it's already clean)
- Block definitions inside will use the rewritten Swiper-free `pcb-block-definition`
- No other changes needed

### 13. `queries/definitions.ts` — API Optimization

**Change:**
- `fetchDefinitionsPerCategory()`: call `fetchAllDefinitions()` and `fetchAllCategories()` with `Promise.all()` instead of sequential `await`

**Keep:**
- All endpoint paths and response types
- Error handling pattern

### 14. Delete `utils/swiper.ts`

No longer needed after Swiper removal.

### 15. Delete `css/swiper.css`

No longer needed.

### 16. `utils/copyPaste.ts` — Add sessionStorage Helpers

**Add:**
- `saveCopiedToSession(data: CopiedData)` — `sessionStorage.setItem('pcb-copy-paste', JSON.stringify(data))`
- `loadCopiedFromSession(): CopiedData | null` — `JSON.parse(sessionStorage.getItem('pcb-copy-paste'))`

**Keep:**
- `differentiateBlocks()` and `regenerateBlockListKeys()` — these are correct and efficient

### 17. Other Utils — No Changes

- `utils/block.ts` — Keep as-is
- `utils/preset.ts` — Keep as-is
- `utils/toast.ts` — Keep as-is

### 18. `events/` — No Changes

All 9 event classes are lightweight and necessary. Keep as-is:
- `events/block.ts`
- `events/copyPaste.ts`
- `events/generic.ts`
- `events/preview.ts`
- `events/toast.ts`

### 19. `index.ts` — Minor Update

- Remove transitive Redux/Swiper imports (they came through barrel imports)
- Keep CSS variable injection

### 20. `components/pcb-preview.ts` — No Changes (Out of Scope)

As agreed, the preview iframe component stays untouched.

### 21. CSS Files — Preserve All Styling

- `css/variables.css` — Keep as-is
- `css/base.css` — Keep as-is
- `css/icon.css` — Keep as-is
- `block-head.css` — Keep as-is
- `pcb-block-spacer.css` — Keep as-is
- `pcb-block-definition.css` — Keep as-is (minor adjustments to remove Swiper-specific rules)
- `addBlockModal.css` — Keep as-is
- `pcb-inline-layout-switch.css` — Update for new non-Swiper layout switcher
- `css/swiper.css` — Delete

---

## Implementation Order

1. Remove dependencies from `package.json`, delete `state/` directory and `utils/swiper.ts`
2. Create new `PcbEditorContext` in `context/`
3. Rewrite `editor/perplex-content-blocks.ts` to use context (no Redux)
4. Rewrite `components/block/pcb-block.ts` to use shared caches (no Redux)
5. Rewrite `components/block/blockHead/pcb-block-head.ts` (no Redux)
6. Rewrite `components/block/blockSpacer/pcb-block-spacer.ts` (no Redux)
7. Rewrite `components/block/inlineLayoutSwitch/pcb-inline-layout-switch.ts` (no Swiper)
8. Rewrite `components/block/blockDefinition/pcb-block-definition.ts` (no Swiper)
9. Update `queries/definitions.ts` (parallel API calls)
10. Update `utils/copyPaste.ts` (sessionStorage helpers)
11. Update `components/dragAndDrop/pcb-drag-item.ts` (event listener cleanup)
12. Update `index.ts` (remove transitive imports)
13. Clean up unused CSS (`swiper.css`, `inline-layout-switch.css` if empty)
14. Build verification (`tsc && vite build`)

---

## Expected Performance Improvements

| Metric | Before | After | Improvement |
|---|---|---|---|
| Component re-renders per state change | 60+ | 1-3 (targeted) | ~95% fewer |
| HTTP requests (20 same-type blocks) | ~83 (3 API + 20 CT + 60 DT) | ~8 (3 API + 1 CT + ~5 DT) | ~90% fewer |
| Bundle size (dependencies) | ~120KB | ~15KB | ~88% smaller |
| Swiper instances per page | 40+ | 0 | 100% eliminated |
| DOM nodes per block | ~50+ | ~15 | ~70% fewer |
| Initial API load time | Sequential | Parallel | ~50% faster |
