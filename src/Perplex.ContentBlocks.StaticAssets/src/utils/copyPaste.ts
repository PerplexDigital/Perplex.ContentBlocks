import { PerplexContentBlocksBlock } from '../types.ts';

type BlockValue = {
    layout?: Record<string, any>;
    contentData?: any[];
    settingsData?: any[];
    expose?: any[];
};

function regenerateBlockListKeys(value: BlockValue | null | undefined): BlockValue | null | undefined {
    if (!value) return value;

    const blockListLayout = (value.layout?.['Umbraco.BlockList'] ?? []) as any[];
    const contentData = value.contentData ?? [];

    if (!blockListLayout.length || !contentData.length) {
        // still return a shallow clone to avoid mutating original object
        return {
            ...value,
            layout: { 'Umbraco.BlockList': blockListLayout },
            contentData,
            expose: value.expose ?? [],
        };
    }

    const keyMap = new Map<string, string>();
    for (const item of blockListLayout) {
        if (item?.contentKey) keyMap.set(item.contentKey, crypto.randomUUID());
    }

    const remapKey = (oldKey: string | null | undefined) => {
        if (!oldKey) return oldKey;
        return keyMap.get(oldKey) ?? oldKey;
    };

    const newLayout = {
        'Umbraco.BlockList': blockListLayout.map((item: any) => ({
            ...item,
            contentKey: remapKey(item?.contentKey),
        })),
    };

    const newContentData = contentData.map((data: any) => {
        const newKey = remapKey(data?.key);
        const originalValues = Array.isArray(data?.values) ? data.values : [];
        const newValues = originalValues.map((val: any) => {
            if (val?.editorAlias === 'Umbraco.BlockList' && val?.value && typeof val.value === 'object') {
                return {
                    ...val,
                    value: regenerateBlockListKeys(val.value),
                };
            }
            return val;
        });

        return {
            ...data,
            key: newKey,
            values: newValues,
        };
    });

    const newExpose = (Array.isArray(value.expose) ? value.expose : []).map((e: any) => ({
        ...e,
        contentKey: remapKey(e?.contentKey),
    }));

    return {
        ...value,
        layout: newLayout,
        contentData: newContentData,
        expose: newExpose,
    };
}

export const differentiateBlocks = (blocks: PerplexContentBlocksBlock[]) => {
    return blocks.map((block) => {
        const originalValues = Array.isArray(block?.content?.values) ? block.content.values : [];
        const newValues = originalValues.map((val: any) => {
            if (val?.editorAlias === 'Umbraco.BlockList' && val?.value && typeof val.value === 'object') {
                return {
                    ...val,
                    value: regenerateBlockListKeys(val.value),
                };
            }
            return val;
        });

        const newContent = {
            ...block.content,
            key: crypto.randomUUID(),
            values: newValues,
        };

        return {
            ...block,
            id: crypto.randomUUID(),
            content: newContent,
        };
    });
};
