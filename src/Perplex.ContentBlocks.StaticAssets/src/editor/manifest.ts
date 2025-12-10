import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/property-editor';

const manifest: ManifestPropertyEditorUi = {
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
                        'Determines whether to show the header, blocks, or both. This affects the back office UI and front end rendering.',
                    propertyEditorUiAlias: 'Perplex.ContentBlocks.Structure',
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

umbExtensionsRegistry.register(manifest);
