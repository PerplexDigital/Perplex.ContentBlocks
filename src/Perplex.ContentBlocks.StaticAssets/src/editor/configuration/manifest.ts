import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/property-editor';

const manifest: ManifestPropertyEditorUi = {
    type: 'propertyEditorUi',
    alias: 'Perplex.ContentBlocks.Structure',
    elementName: 'perplex-content-blocks-structure',

    // Everything else is unused since this editor is only used as a configuration editor and will not show up in the property editor UI picker modal.
    name: '',
    meta: {
        label: '',
        icon: '',
        group: '',
        propertyEditorSchemaAlias: undefined, // By setting it to undefined, this editor won't appear in the property editor UI picker modal.
    },
};

umbExtensionsRegistry.register(manifest);
