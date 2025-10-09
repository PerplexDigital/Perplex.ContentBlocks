export const ON_VALUE_COPIED = 'ON_VALUE_COPIED';
export const ON_VALUE_PASTE = 'ON_VALUE_PASTE';
import { PerplexContentBlocksBlock, Section } from '../types.ts';

export const ValueCopiedEvent = (copiedValue: PerplexContentBlocksBlock[]) =>
    new CustomEvent(ON_VALUE_COPIED, {
        detail: copiedValue,
        bubbles: true,
        cancelable: true,
        composed: true,
    });

export const ValuePastedEvent = (pastedValue: PerplexContentBlocksBlock[], section: Section, desiredIndex?: number) =>
    new CustomEvent(ON_VALUE_PASTE, {
        detail: {
            pastedValue,
            desiredIndex,
            section,
        },
        bubbles: true,
        cancelable: true,
        composed: true,
    });
