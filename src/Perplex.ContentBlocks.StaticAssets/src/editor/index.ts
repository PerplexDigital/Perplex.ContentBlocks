import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';

import './perplex-content-blocks';
import { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/property-editor';

const extension: ManifestPropertyEditorUi = {
    type: 'propertyEditorUi',
    alias: 'Perplex.ContentBlocks',
    name: 'Perplex.ContentBlocks',
    elementName: 'perplex-content-blocks',
    meta: {
        label: 'Perplex.ContentBlocks',
        icon: 'icon-thumbnail-list',
        group: 'lists',
        propertyEditorSchemaAlias: 'Perplex.ContentBlocks',
        settings: {
            properties: [
                {
                    alias: 'structure',
                    label: 'Structure',
                    description:
                        'Determines whether to show the header, blocks, or both. The values are `Header`, `Blocks`, or `All`, respectively. This affects the back office UI and front end rendering.',
                    propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
                },
                {
                    alias: 'debug',
                    label: 'Debug',
                    description: 'Shows debug information in the backoffice',
                    propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
                },
            ],
            defaultData: [
                {
                    alias: 'version',
                    value: 4,
                },
                {
                    alias: 'structure',
                    value: 'All',
                },
                {
                    alias: 'debug',
                    value: false,
                },
            ],
        },
    },
};

umbExtensionsRegistry.register(extension);
