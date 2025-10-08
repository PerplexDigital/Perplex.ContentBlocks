import type { UmbBlockDataModel } from '@umbraco-cms/backoffice/block';

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
