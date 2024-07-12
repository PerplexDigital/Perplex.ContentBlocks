import type {UmbBlockDataType} from "@umbraco-cms/backoffice/block";

export interface PerplexBlockDefinition {
    id:                   string;
    name:                 string;
    description:          string;
    blockNameTemplate:    string;
    previewImage:         string;
    elementTypeKey:       string;
    categoryIds:          string[];
    layouts:              Layout[];
    limitToDocumentTypes: any[];
    limitToCultures:      any[];
}

export interface Layout {
    id:           string;
    name:         string;
    description:  null;
    previewImage: string;
}

export type PerplexContentBlocksValue = {
    version: number;
    header: PerplexContentBlocksBlock | null;
    blocks: PerplexContentBlocksBlock[];
};

export type PerplexContentBlocksBlock = {
    id: string;
    definitionId: string;
    layoutId: string;
    presetId?: string;
    isDisabled: boolean;
    content: UmbBlockDataType;
    variants?: PerplexContentBlocksBlockVariant[];
};

export type PerplexContentBlocksBlockVariant = {
    id: string;
    alias: string;
    content: UmbBlockDataType;
};

export type PerplexContentBlocksBlockOnChangeFn = (block: PerplexContentBlocksBlock) => void;