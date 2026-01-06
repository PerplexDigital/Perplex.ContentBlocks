import type { UmbBlockDataModel } from '@umbraco-cms/backoffice/block';
import { UmbPropertyTypeModel } from '@umbraco-cms/backoffice/content-type';

export interface PerplexBlockDefinition {
    id: string;
    name: string;
    description: string;
    blockNameTemplate: string;
    icon: string;
    previewImage: string;
    elementTypeKey: string;
    categoryIds: string[];
    layouts: Layout[];
    limitToDocumentTypes: any[];
    limitToCultures: any[];
}

export interface PCBCategory {
    id: string;
    name: string;
    icon: string;
    isHidden: boolean;
    isEnabledForHeaders: boolean;
    isDisabledForBlocks: boolean;
}

export interface DefinitionsDictionary {
    [key: string]: PerplexBlockDefinition;
}

export interface PCBCategoryWithDefinitions {
    category: PCBCategory;
    definitions: DefinitionsDictionary;
}

export interface Layout {
    id: string;
    name: string;
    description: null;
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
    content: UmbBlockDataModel;
};

export type PerplexContentBlocksBlockOnChangeFn = (block: PerplexContentBlocksBlock) => void;

export enum Section {
    HEADER,
    CONTENT,
}

export enum Structure {
    All = 'All',
    Blocks = 'Blocks',
    Header = 'Header',
}

export interface PcbDragItemElement extends HTMLElement {
    blockId: string;
}

export type PresetItem = {
    id: string;
    definitionId: string;
    layoutId: string;
    isMandatory: boolean;
    values: {
        [key: string]: unknown;
    };
};

export type Preset = {
    id: string;
    name: string;
    applyToCultures: string[];
    applyToDocumentTypes: string[];
    header: PresetItem;
    blocks: PresetItem[];
};

export interface Tab {
    id: string;
    name: string;
    sortOrder: number;
    groups: Group[];
    properties: UmbPropertyTypeModel[];
}

export interface Group {
    id: string;
    name: string;
    sortOrder: number;
    properties: UmbPropertyTypeModel[];
}
