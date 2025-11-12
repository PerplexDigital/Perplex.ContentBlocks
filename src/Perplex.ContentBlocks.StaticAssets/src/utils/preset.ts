import {
    PCBCategoryWithDefinitions,
    PerplexContentBlocksBlock,
    PerplexContentBlocksValue,
    Preset,
    PresetItem,
} from '../types.ts';
import { UmbId } from '@umbraco-cms/backoffice/id';

export const getBlocksFromPreset = (
    preset: Preset,
    definitions: PCBCategoryWithDefinitions[],
    currentValue: PerplexContentBlocksValue,
) => {
    let returnVal = {
        header: null as PerplexContentBlocksValue['header'],
        blocks: [] as PerplexContentBlocksValue['blocks'],
    };

    if (preset && definitions) {
        if (preset.header && !currentValue.header) {
            returnVal.header = makeBlockFromPresetItem(preset.header, definitions);
        }

        if (preset.blocks && currentValue.blocks.length === 0) {
            returnVal.blocks = preset.blocks
                .map((item) => makeBlockFromPresetItem(item, definitions))
                .filter((block): block is PerplexContentBlocksBlock => block !== null);
        }
    }

    return returnVal;
};

const makeBlockFromPresetItem = (
    presetItem: PresetItem,
    definitions: PCBCategoryWithDefinitions[],
): PerplexContentBlocksBlock | null => {
    const definition = definitions
        .map((cat) => cat.definitions[presetItem.definitionId])
        .find((def) => def !== undefined);

    if (!definition) return null;

    let values;

    if (presetItem.values && typeof presetItem.values === 'object') {
        values = Object.entries(presetItem.values).map(([key, value]) => {
            return {
                alias: key,
                editorAlias: '',
                culture: null,
                segment: null,
                entityType: '',
                value: value,
            };
        });
    }

    return {
        isDisabled: false,
        definitionId: presetItem.definitionId,
        presetId: presetItem.id,
        layoutId: presetItem.layoutId,
        id: crypto.randomUUID(),
        content: {
            key: UmbId.new(),
            contentTypeKey: definition.elementTypeKey,
            values: values || [],
        },
    };
};
