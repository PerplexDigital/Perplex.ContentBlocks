import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { html, customElement, property, repeat, css, state, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";
import { UmbPropertyValueChangeEvent } from "@umbraco-cms/backoffice/property-editor";
import { type UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import { UmbId } from "@umbraco-cms/backoffice/id";
import type { UmbPropertyTypeModel } from "@umbraco-cms/backoffice/content-type";
import type { UmbBlockDataType } from "@umbraco-cms/backoffice/block";
import { UmbDataPathPropertyValueFilter } from "@umbraco-cms/backoffice/validation";
import { UMB_PROPERTY_CONTEXT } from "@umbraco-cms/backoffice/property";
import { UMB_PROPERTY_DATASET_CONTEXT } from "@umbraco-cms/backoffice/property";

// TODO: Use dynamic ids from endpoint /umbraco/perplex-content-blocks/api/definitions/all
const BLOCK_ELEMENT_TYPE_KEY = "65217f3b-78f5-44d6-8af1-7eb675fbaef0";
const DEFINITION_ID = "34269420-0bb9-48a4-b868-64698a05f6e1";
const LAYOUT_ID = "31fda9e9-d960-4a26-8b39-54d1a5a9e5be";

export type PerplexContentBlocksValue = {
    version: number;
    header: PerplexContentBlocksBlock | null;
    blocks: PerplexContentBlocksBlock[];
};

export type PerplexContentBlocksBlock = {
    id: string;
    definitionId: string;
    layoutId: string;
    presetId?: string;
    isDisabled: boolean;
    content: UmbBlockDataType;
    variants?: PerplexContentBlocksBlockVariant[];
};

export type PerplexContentBlocksBlockVariant = {
    id: string;
    alias: string;
    content: UmbBlockDataType;
};

export type PerplexContentBlocksBlockOnChangeFn = (block: PerplexContentBlocksBlock) => void;

@customElement("perplex-content-blocks")
export default class PerplexContentBlocksElement extends UmbLitElement implements UmbPropertyEditorUiElement {
    @state()
    properties: UmbPropertyTypeModel[] | undefined = undefined;

    @state()
    showDebug: boolean = false;

    @state()
    private _value: PerplexContentBlocksValue = {
        version: 4,
        header: null,
        blocks: [],
    };

    @property({ attribute: false })
    public set value(value: PerplexContentBlocksValue | undefined) {
        if (value == null) {
            return;
        }

        this._value = value;
    }

    public get value(): PerplexContentBlocksValue {
        return this._value;
    }

    @property({ attribute: false })
    config: UmbPropertyEditorConfigCollection | undefined;

    @state()
    dataPath!: string;

    @state()
    pageId!: string;

    @state()
    culture!: string | null;

    constructor() {
        super();

        this.consumeContext(UMB_PROPERTY_CONTEXT, ctx => {
            const alias = ctx.getAlias() || "";
            const culture = ctx.getVariantId()?.culture;
            this.dataPath = `$.values[${UmbDataPathPropertyValueFilter({ alias, culture })}].value`;
        });

        this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, ctx => {
            this.pageId = ctx.getUnique()!;
            this.culture = ctx.getVariantId().culture;
        });
    }

    valueChanged() {
        this.dispatchEvent(new UmbPropertyValueChangeEvent());
    }

    addHeader() {
        const header = this.createBlock(BLOCK_ELEMENT_TYPE_KEY);
        this.updateHeader(header);
        this.valueChanged();
    }

    updateHeader(header: PerplexContentBlocksBlock) {
        this._value = { ...this._value, header };
        this.valueChanged();
    }

    removeHeader() {
        this._value = { ...this._value, header: null };
        this.valueChanged();
    }

    addBlock() {
        // TODO: Dynamic content type
        const block = this.createBlock(BLOCK_ELEMENT_TYPE_KEY);

        const blocks = [...this._value.blocks, block];
        this._value = { ...this._value, blocks };

        this.valueChanged();
    }

    createUdi(entityType: string) {
        return `umb://${entityType}/${UmbId.new().replace(/-/g, "")}`;
    }

    createBlock(contentTypeKey: string): PerplexContentBlocksBlock {
        const udi = this.createUdi("element");
        return {
            id: UmbId.new(),
            definitionId: DEFINITION_ID,
            layoutId: LAYOUT_ID,
            isDisabled: false,
            content: {
                udi: udi,
                contentTypeKey: contentTypeKey,
            },
        };
    }

    removeBlock(contentUdi: string) {
        const blocks = this._value.blocks.filter(block => block.content.udi !== contentUdi);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    updateBlock(block: PerplexContentBlocksBlock) {
        const idx = this._value.blocks.findIndex(b => b.id === block.id);
        if (idx === -1) {
            return;
        }

        const blocks = [...this._value.blocks];
        blocks.splice(idx, 1, block);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    render() {
        return html`<div class="main">
                <h2>Editor</h2>

                ${(!this._value.header &&
                    html`<uui-button look="primary" label="Add header" @click=${this.addHeader}></uui-button>`) ||
                nothing}

                <div class="blocks">
                    ${(this._value.header &&
                        html` <perplex-content-blocks-block
                            .block=${this._value.header}
                            .removeBlock=${this.removeHeader.bind(this)}
                            .onChange=${this.updateHeader.bind(this)}
                            .dataPath=${this.dataPath}
                        ></perplex-content-blocks-block>`) ||
                    nothing}
                    ${repeat(
                        this._value.blocks,
                        block => block.id,
                        block =>
                            html`<perplex-content-blocks-block
                                .block=${block}
                                .removeBlock=${this.removeBlock.bind(this)}
                                .onChange=${this.updateBlock.bind(this)}
                                .dataPath=${this.dataPath}
                            ></perplex-content-blocks-block>`
                    )}
                </div>

                <uui-button look="primary" label="Add block" @click=${this.addBlock}></uui-button>

                <div class="debug">
                    <uui-button
                        look="outline"
                        label="raw value"
                        @click=${() => (this.showDebug = !this.showDebug)}
                    ></uui-button>
                    ${(this.showDebug && html`<pre>${JSON.stringify(this.value, null, 4)}</pre>`) || null}
                </div>
            </div>
            <div class="sidebar">
                <h2>Sidebar</h2>
                <perplex-content-blocks-preview
                    .culture=${this.culture}
                    .pageId=${this.pageId}
                ></perplex-content-blocks-preview>
            </div>`;
    }

    static styles = [
        css`
            :host {
                display: grid;
                grid-template-columns: 3fr 1fr;
                gap: 1rem;
            }

            .main,
            .sidebar {
                padding: 1rem 1.5rem;
            }

            .main {
                border: 4px solid #3465a4;
            }

            .sidebar {
                border: 4px solid #75507b;
            }

            .blocks {
                display: grid;
                grid-template-columns: 1fr;
                gap: 1rem;
                padding-block-end: 1rem;
            }

            .debug {
                margin-top: 1rem;
            }
        `,
    ];
}
