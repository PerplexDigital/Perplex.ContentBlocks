import type { UmbPropertyDatasetContext, UmbPropertyValueData } from '@umbraco-cms/backoffice/property';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UmbVariantId } from '@umbraco-cms/backoffice/variant';
import type { UmbBlockDataValueModel } from '@umbraco-cms/backoffice/block';
import {
    Observable,
    UmbArrayState,
    UmbBooleanState,
    UmbClassState,
    UmbStringState,
} from '@umbraco-cms/backoffice/observable-api';
import { create } from 'mutative';
import type { PerplexContentBlocksBlock, PerplexContentBlocksBlockOnChangeFn } from '../types.js';
import { UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import { propertyAliasPrefix } from '../utils/block.js';

export class PerplexContentBlocksPropertyDatasetContext extends UmbControllerBase implements UmbPropertyDatasetContext {
    #block: PerplexContentBlocksBlock;
    #properties: UmbArrayState<UmbBlockDataValueModel>;
    #variantId: UmbClassState<UmbVariantId>;
    #PROPERTY_ALIAS_PREFIX_LENGTH: number;
    #onChange: (block: PerplexContentBlocksBlock) => void;

    constructor(
        host: UmbControllerHost,
        block: PerplexContentBlocksBlock,
        onChange: PerplexContentBlocksBlockOnChangeFn,
    ) {
        super(host);

        this.#block = block;
        this.#onChange = onChange;

        this.#properties = new UmbArrayState(block.content.values, (p) => p.alias + '#' + p.culture + '#' + p.segment);
        this.#variantId = new UmbClassState(UmbVariantId.CreateInvariant());

        this.#PROPERTY_ALIAS_PREFIX_LENGTH = propertyAliasPrefix(this.#block).length;

        this.name = new UmbStringState(this.getName()).asObservable();
        this.properties = this.#properties.asObservable();
        this.readOnly = new UmbBooleanState(this.getReadOnly()).asObservable();

        this.provideContext(UMB_PROPERTY_DATASET_CONTEXT.contextAlias, this);
    }

    getEntityType() {
        return 'element';
    }

    getName() {
        return 'TODO: Block';
    }

    getProperties(): Promise<Array<UmbPropertyValueData> | undefined> {
        return Promise.resolve(this.#properties.getValue());
    }

    getReadOnly(): boolean {
        return false;
    }

    getUnique() {
        return this.#block.id;
    }

    getVariantId() {
        return this.#variantId.getValue();
    }

    readonly name: Observable<string | undefined>;
    readonly properties: Observable<Array<UmbPropertyValueData>>;
    readonly readOnly: Observable<boolean>;
    readonly propertyVariantId? = () => Promise.resolve(this.#variantId.asObservable());

    cleanAlias(alias: string): string {
        return alias.substring(this.#PROPERTY_ALIAS_PREFIX_LENGTH);
    }

    async propertyValueByAlias<ReturnType = unknown>(propertyAlias: string) {
        const alias = this.cleanAlias(propertyAlias);
        return this.#properties.asObservablePart((props) => {
            if (!Array.isArray(props)) return undefined;
            return props.find((prop) => prop.alias === alias && prop.culture == null && prop.segment == null)
                ?.value as ReturnType;
        });
    }

    async setPropertyValue(propertyAlias: string, value: unknown) {
        const alias = this.cleanAlias(propertyAlias);

        const values = create(this.#properties.getValue() ?? [], (props) => {
            let item = props.find((prop) => prop.alias === alias && prop.culture == null && prop.segment == null);

            if (item == null) {
                item = { alias, editorAlias: '', culture: null, segment: null, entityType: '' };
                props.push(item);
            }

            item.value = value;
        });

        this.#properties.setValue(values);

        this.#block = create(this.#block, (block) => {
            block.content.values = values;
        });

        this.#onChange(this.#block);
    }
}
