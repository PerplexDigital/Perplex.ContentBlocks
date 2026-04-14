import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbDataTypeDetailModel, UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbDocumentTypeDetailModel, UmbDocumentTypeDetailRepository } from '@umbraco-cms/backoffice/document-type';
import type {
    PCBCategoryWithDefinitions,
    PerplexBlockDefinition,
    PerplexContentBlocksBlock,
    Preset,
} from '../types.ts';

// ── Copy/Paste types ────────────────────────────────────────────────

export type CopiedData = {
    header: PerplexContentBlocksBlock | null;
    blocks: PerplexContentBlocksBlock[];
};

const COPY_PASTE_STORAGE_KEY = 'pcb-copy-paste';

// ── Context class ───────────────────────────────────────────────────

/**
 * Central editor context shared via `@lit/context` from the main editor to all
 * child components. Replaces the old Redux store + pwa-helpers connect approach
 * with targeted, granular data sharing and shared caches for HTTP deduplication.
 */
export class PcbEditorContext {
    readonly editorId: string;

    // ── Static data (set once after fetch, not expected to change) ────
    definitions: PCBCategoryWithDefinitions[] = [];
    definitionsMap: Map<string, PerplexBlockDefinition> = new Map();
    headerCategories: string[] = [];
    presets: Preset | null = null;

    // ── UI state ─────────────────────────────────────────────────────
    isTouchDevice: boolean = false;

    // ── Shared caches (deduplicate HTTP requests across blocks) ──────
    readonly #contentTypeCache = new Map<string, Promise<UmbDocumentTypeDetailModel>>();
    readonly #dataTypeCache = new Map<string, Promise<UmbDataTypeDetailModel>>();
    readonly #contentTypeRepo: UmbDocumentTypeDetailRepository;
    readonly #dataTypeRepo: UmbDataTypeDetailRepository;

    constructor(host: UmbControllerHost) {
        this.editorId = crypto.randomUUID();
        this.#contentTypeRepo = new UmbDocumentTypeDetailRepository(host);
        this.#dataTypeRepo = new UmbDataTypeDetailRepository(host);
        this.isTouchDevice = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
    }

    // ── Definitions helpers ──────────────────────────────────────────

    setDefinitions(defs: PCBCategoryWithDefinitions[]) {
        this.definitions = defs;
        this.definitionsMap.clear();
        this.headerCategories = [];

        for (const cat of defs) {
            if (cat.category.isEnabledForHeaders) {
                this.headerCategories.push(cat.category.id);
            }
            for (const [id, def] of Object.entries(cat.definitions)) {
                this.definitionsMap.set(id, def);
            }
        }
    }

    findDefinitionById(id: string): PerplexBlockDefinition | null {
        return this.definitionsMap.get(id) ?? null;
    }

    // ── Copy / Paste (sessionStorage-backed) ─────────────────────────

    getCopied(): CopiedData | null {
        try {
            const raw = sessionStorage.getItem(COPY_PASTE_STORAGE_KEY);
            return raw ? (JSON.parse(raw) as CopiedData) : null;
        } catch {
            return null;
        }
    }

    setCopied(data: CopiedData | null) {
        if (data == null) {
            sessionStorage.removeItem(COPY_PASTE_STORAGE_KEY);
        } else {
            sessionStorage.setItem(COPY_PASTE_STORAGE_KEY, JSON.stringify(data));
        }
    }

    // ── Shared content-type cache ────────────────────────────────────

    /**
     * Returns a cached promise for the content type. If another block already
     * requested this content type, the same in-flight promise is reused
     * (deduplicating both the HTTP call and the Umbraco repository overhead).
     */
    getContentType(key: string): Promise<UmbDocumentTypeDetailModel> {
        let cached = this.#contentTypeCache.get(key);
        if (cached) return cached;

        cached = this.#contentTypeRepo.requestByUnique(key).then(res => {
            if (res.data == null) {
                this.#contentTypeCache.delete(key);
                throw new Error(`Cannot retrieve content type ${key}`);
            }
            return res.data;
        });

        this.#contentTypeCache.set(key, cached);
        return cached;
    }

    // ── Shared data-type cache ───────────────────────────────────────

    /**
     * Same deduplication strategy as content types.
     */
    getDataType(key: string): Promise<UmbDataTypeDetailModel> {
        let cached = this.#dataTypeCache.get(key);
        if (cached) return cached;

        cached = this.#dataTypeRepo.requestByUnique(key).then(res => {
            if (res.data == null) {
                this.#dataTypeCache.delete(key);
                throw new Error(`Cannot retrieve data type ${key}`);
            }
            return res.data;
        });

        this.#dataTypeCache.set(key, cached);
        return cached;
    }
}
