import { PerplexContentBlocksBlock, Section } from '../types.ts';
import { CopiedData } from '../state/slices/copyPaste.ts';

export class PcbValueCopiedEvent extends Event {
    public static readonly TYPE = 'PcbValueCopiedEvent';
    public readonly blocks: PerplexContentBlocksBlock[];
    public readonly section: Section;

    public constructor(blocks: PerplexContentBlocksBlock[], section: Section) {
        super(PcbValueCopiedEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.blocks = blocks;
        this.section = section;
    }
}

export class PcbValuePastedEvent extends Event {
    public static readonly TYPE = 'PcbValuePastedEvent';
    public readonly pastedValue: CopiedData;
    public readonly section: Section;
    public readonly desiredIndex?: number;

    public constructor(pastedValue: CopiedData, section: Section, desiredIndex?: number) {
        super(PcbValuePastedEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.pastedValue = pastedValue;
        this.section = section;
        this.desiredIndex = desiredIndex;
    }
}
