import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { PCBCategoryWithDefinitions, PerplexContentBlocksBlock, Section } from '../../../types.ts';

export type PcbAddBlockModalData = {
    editorId: string;
    section: Section;
    groupedDefinitions: PCBCategoryWithDefinitions[];
    insertAtIndex?: number;
};

export type PcbAddBlockModalValue = {
    blocks: PerplexContentBlocksBlock[];
    section: Section;
    desiredIndex: number | null;
};

export const PCB_ADD_BLOCK_MODAL_TOKEN = new UmbModalToken<PcbAddBlockModalData, PcbAddBlockModalValue>(
    'Pcb.AddBlock',
    {
        modal: {
            type: 'sidebar',
            size: 'large',
        },
    },
);
