import { PerplexBlockDefinition, PerplexContentBlocksBlock, Section } from '../types.ts';

export type BlockCreation = {
    blocks: PerplexContentBlocksBlock[];
    section: Section;
    desiredIndex: number | null;
};

export class PcbBlockSavedEvent extends Event {
    public static readonly TYPE = 'PcbBlockSavedEvent';
    public readonly blocks: PerplexContentBlocksBlock[];
    public readonly section: Section;
    public readonly desiredIndex: number | null;

    public constructor(blockCreation: BlockCreation) {
        super(PcbBlockSavedEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.blocks = blockCreation.blocks;
        this.section = blockCreation.section;
        this.desiredIndex = blockCreation.desiredIndex;
    }
}

export class PcbBlockToggleEvent extends Event {
    public static readonly TYPE = 'PcbBlockToggleEvent';
    public readonly id: string;

    public constructor(id: string) {
        super(PcbBlockToggleEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.id = id;
    }
}

export class PcbBlockUpdatedEvent extends Event {
    public static readonly TYPE = 'PcbBlockUpdatedEvent';
    public readonly block: PerplexContentBlocksBlock;
    public readonly definition: PerplexBlockDefinition;
    public readonly section: Section;
    public editorId?: string;

    public constructor(
        block: PerplexContentBlocksBlock,
        definition: PerplexBlockDefinition,
        section: Section,
        editorId?: string,
    ) {
        super(PcbBlockUpdatedEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.block = block;
        this.definition = definition;
        this.section = section;
        this.editorId = editorId;
    }
}

export class PcbBlockLayoutChangeEvent extends Event {
    public static readonly TYPE = 'PcbBlockLayoutChangeEvent';
    public readonly selectedLayout: { id: string; name: string };

    public constructor(selectedLayout: { id: string; name: string }) {
        super(PcbBlockLayoutChangeEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.selectedLayout = selectedLayout;
    }
}

export class PcbSetBlocksEvent extends Event {
    public static readonly TYPE = 'PcbSetBlocksEvent';
    public readonly blocks: PerplexContentBlocksBlock[];

    public constructor(blocks: PerplexContentBlocksBlock[]) {
        super(PcbSetBlocksEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.blocks = blocks;
    }
}

export const ON_BLOCK_HEAD_CLICK = 'ON_BLOCK_HEAD_CLICK';
export const ON_BLOCK_SELECTED = 'ON_BLOCK_SELECTED';
export const ON_BLOCK_REMOVE = 'ON_BLOCK_REMOVE';
