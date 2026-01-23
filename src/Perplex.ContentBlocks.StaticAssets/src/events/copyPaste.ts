export const ON_VALUE_COPIED = 'ON_VALUE_COPIED';
export const ON_VALUE_PASTE = 'ON_VALUE_PASTE';
import { PerplexContentBlocksBlock, Section } from '../types.ts';
import { CopiedData } from '../state/slices/copyPaste.ts';

export type CopiedEventDetail = {
    blocks: PerplexContentBlocksBlock[];
    section: Section;
};

export const ValueCopiedEvent = (copiedValue: PerplexContentBlocksBlock[], section: Section) =>
    new CustomEvent<CopiedEventDetail>(ON_VALUE_COPIED, {
        detail: { blocks: copiedValue, section },
        bubbles: true,
        cancelable: true,
        composed: true,
    });

export type PastedEventDetail = {
    pastedValue: CopiedData;
    section: Section;
    desiredIndex?: number;
};

export const ValuePastedEvent = (pastedValue: CopiedData, section: Section, desiredIndex?: number) =>
    new CustomEvent<PastedEventDetail>(ON_VALUE_PASTE, {
        detail: {
            pastedValue,
            desiredIndex,
            section,
        },
        bubbles: true,
        cancelable: true,
        composed: true,
    });
