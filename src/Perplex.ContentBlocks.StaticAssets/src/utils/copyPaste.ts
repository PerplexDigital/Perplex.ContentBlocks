import { PerplexContentBlocksBlock } from '../types.ts';

type BlockValue = {
    layout?: Record<string, any>;
    contentData?: any[];
    settingsData?: any[];
    expose?: any[];
};

function regenerateBlockListKeys(value: BlockValue): BlockValue {
    if (!value?.layout || !value?.contentData) return value;

    // Map old contentKey → new UUID
    const keyMap = new Map<string, string>();
    for (const item of value.layout['Umbraco.BlockList'] ?? []) {
        if (item.contentKey) keyMap.set(item.contentKey, crypto.randomUUID());
    }

    const remapKey = (oldKey: string | null | undefined) => {
        if (!oldKey) return oldKey;
        return keyMap.get(oldKey) ?? oldKey;
    };

    // Replace keys in layout
    const newLayout = {
        'Umbraco.BlockList': value.layout['Umbraco.BlockList'].map((item: any) => ({
            ...item,
            contentKey: remapKey(item.contentKey),
        })),
    };

    // Replace keys + recurse inside contentData
    const newContentData = value.contentData.map((data: any) => {
        const newKey = remapKey(data.key);
        const newValues =
            data.values?.map((val: any) => {
                if (val.editorAlias === 'Umbraco.BlockList' && val.value) {
                    return {
                        ...val,
                        value: regenerateBlockListKeys(val.value),
                    };
                }
                return val;
            }) ?? [];

        return {
            ...data,
            key: newKey,
            values: newValues,
        };
    });

    // Replace keys in expose (if present)
    const newExpose =
        value.expose?.map((e: any) => ({
            ...e,
            contentKey: remapKey(e.contentKey),
        })) ?? [];

    return {
        ...value,
        layout: newLayout,
        contentData: newContentData,
        expose: newExpose,
    };
}

export const differentiateBlocks = (blocks: PerplexContentBlocksBlock[]) => {
    return blocks.map((block) => {
        const newContent = {
            ...block.content,
            key: crypto.randomUUID(),
            values: block.content.values.map((val: any) => {
                if (val.editorAlias === 'Umbraco.BlockList' && val.value) {
                    return {
                        ...val,
                        value: regenerateBlockListKeys(val.value),
                    };
                }
                return val;
            }),
        };

        return {
            ...block,
            id: crypto.randomUUID(),
            content: newContent,
        };
    });
};
