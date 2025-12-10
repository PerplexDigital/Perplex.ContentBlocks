import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import type { ManifestModal } from '@umbraco-cms/backoffice/modal';
import { ELEMENT_NAME } from './pcb-add-block-modal.ts';
import { MODAL_ALIAS } from './modal-token';

const extension: ManifestModal = {
    type: 'modal',
    alias: MODAL_ALIAS,
    name: 'Content Blocks add Block modal',
    elementName: ELEMENT_NAME,
};

umbExtensionsRegistry.register(extension);
