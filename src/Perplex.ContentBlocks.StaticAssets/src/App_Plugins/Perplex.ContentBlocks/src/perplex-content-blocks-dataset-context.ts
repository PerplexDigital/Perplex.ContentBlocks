import type { UmbPropertyDatasetContext } from "@umbraco-cms/backoffice/property";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbVariantId } from "@umbraco-cms/backoffice/variant";
import type { UmbBlockDataModel } from "@umbraco-cms/backoffice/block";
import type { PerplexContentBlocksBlock, PerplexContentBlocksBlockOnChangeFn } from "./perplex-content-blocks.js";
import { Observable, UmbBooleanState, UmbObjectState, UmbStringState } from "@umbraco-cms/backoffice/observable-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { create } from "mutative";

export class PerplexContentBlocksPropertyDatasetContext extends UmbControllerBase implements UmbPropertyDatasetContext {
    #block: PerplexContentBlocksBlock;
    #content: UmbObjectState<UmbBlockDataModel>;
    #token = new UmbContextToken<PerplexContentBlocksPropertyDatasetContext>("UmbPropertyDatasetContext");
    #onChange: (block: PerplexContentBlocksBlock) => void;

    getVariantId() {
        return UmbVariantId.CreateInvariant();
    }
    getEntityType() {
        return "element";
    }
    getUnique() {
        return this.#block.id;
    }

    getName(): string | undefined {
        return "TODO: ContentBlocks name";
    }

    constructor(
        host: UmbControllerHost,
        block: PerplexContentBlocksBlock,
        onChange: PerplexContentBlocksBlockOnChangeFn
    ) {
        super(host);

        this.#block = block;
        this.#content = new UmbObjectState<UmbBlockDataModel>(this.#block.content);
        this.#onChange = onChange;

        this.provideContext(this.#token, this);
    }
    getReadOnly(): boolean {
        throw new Error("Method not implemented.");
    }

    readOnly: Observable<boolean> = new UmbBooleanState(false).asObservable();

    readonly name: Observable<string | undefined> = new UmbStringState(this.getName()).asObservable();

    propertyVariantId?: ((propertyAlias: string) => Promise<Observable<UmbVariantId | undefined>>) | undefined;

    async propertyValueByAlias<ReturnType = unknown>(propertyAlias: string) {
        return this.#content.asObservablePart(c => c.values.find(v => v.alias === propertyAlias)?.value as ReturnType);
    }

    async setPropertyValue(propertyAlias: string, value: unknown) {
        const updated = create(this.#content.getValue(), content => {
            let item = content.values.find(v => v.alias === propertyAlias);
            if (item == null) {
                item = { alias: propertyAlias, editorAlias: "", culture: null, segment: null };
                content.values.push(item);
            }

            item.value = value;
        });
        this.#content.setValue(updated);
        this.#block = { ...this.#block, content: updated };
        this.#onChange(this.#block);
    }
}
