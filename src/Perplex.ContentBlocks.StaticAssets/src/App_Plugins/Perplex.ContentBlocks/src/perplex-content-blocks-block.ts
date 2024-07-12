import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { html, repeat, customElement, property, css, state } from "@umbraco-cms/backoffice/external/lit";
import type { UmbPropertyTypeModel } from "@umbraco-cms/backoffice/content-type";
import { PerplexContentBlocksPropertyDatasetContext } from "./perplex-content-blocks-dataset-context";
import { UmbDocumentTypeDetailModel, UmbDocumentTypeDetailRepository } from "@umbraco-cms/backoffice/document-type";
import { UMB_VALIDATION_CONTEXT } from "@umbraco-cms/backoffice/validation";
import { UmbValidationContext } from "@umbraco-cms/backoffice/validation";
import {variables} from "./styles.ts";
import contentBlockName from "./utils/contentBlockName.ts";
import {PerplexContentBlocksBlock, PerplexContentBlocksBlockOnChangeFn} from "./types.ts";
import {ON_BLOCK_HEAD_CLICK} from "./events/block.ts";

@customElement("perplex-content-blocks-block")
export default class PerplexContentBlocksBlockElement extends UmbLitElement {
    @state()
    private ok: boolean = false;

    #contentTypeRepository = new UmbDocumentTypeDetailRepository(this);
    elementType!: UmbDocumentTypeDetailModel;

    @state()
    collapsed: boolean = true;

    @state()
    properties: UmbPropertyTypeModel[] | undefined = undefined;

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

    @property({ attribute: false })
    removeBlock!: (udi: string) => void;

    @property({ attribute: false })
    onChange!: PerplexContentBlocksBlockOnChangeFn;

    @property({ attribute: false })
    dataPath!: string;

    #validationContext!: UmbValidationContext;

    connectedCallback() {
        super.connectedCallback();
        const errors: string[] = [];

        if (!this.block) {
            errors.push("block property is required");
        }

        if (!this.removeBlock) {
            errors.push("removeBlock property is required");
        }

        if (!this.removeBlock) {
            errors.push("onChange property is required");
        }

        if (errors.length > 0) {
            throw new Error(errors.join(" | "));
        }
    }

    disconnectedCallback() {
        super.disconnectedCallback();

        this.clearValidationMessages();
    }

    constructor() {
        super();
        this.consumeContext(UMB_VALIDATION_CONTEXT, ctx => {
            this.#validationContext = ctx;
        });

        this.addEventListener(ON_BLOCK_HEAD_CLICK, this.onHeadClicked)
    }



    onHeadClicked = () => {
        this.collapsed = !this.collapsed;
    }

    async firstUpdated() {
        const elementTypeResponse = await this.#contentTypeRepository.requestByUnique(
            this.block.content.contentTypeKey
        );
        if (elementTypeResponse.data == null) {
            throw new Error(`Cannot retrieve content type ${this.block.content.contentTypeKey}`);
        }

        this.elementType = elementTypeResponse.data;
        new PerplexContentBlocksPropertyDatasetContext(this, this.block, this.onChange);

        this.ok = true;
    }

    clearValidationMessages() {
        for (const property of this.elementType.properties) {
            const path = `${this.dataPath}.${this.block.id}.${property.alias}`;
            this.#validationContext.messages.removeMessagesByTypeAndPath("client", path);
            this.#validationContext.messages.removeMessagesByTypeAndPath("server", path);
        }
    }

    render() {
        if (!this.ok) {
            return;
        }

        return html`<div class="block">
            <pcb-block-head
                .blockDefinitionName=${this.elementType.name}
                .icon=${this.elementType.icon}
                .collapsed="${this.collapsed}"
                .blockTemplateName="${contentBlockName('', {})}"
            >
            </pcb-block-head>
            <div class=${this.collapsed ? 'block__body block__body--hidden' : 'block__body block__body--open'}>
                <div>
                    ${repeat(
                            this.elementType.properties,
                            property => property.id,
                            property =>
                                    html`<umb-property-type-based-property
                            .property=${property}
                            .dataPath=${`${this.dataPath}.${this.block.id}.${property.alias}`}
                        ></umb-property-type-based-property>`
                    )}
                    <button type="button" class="remove-block" @click=${this.removeBlock.bind(null, this.block.content.udi)}>
                        &times;
                    </button>
                </div>
            </div>
        </div>`;
    }

    static styles = [
        variables,
        css`
            .block {
                box-shadow: var(--bs-base);

                .block__body {
                    background-color: var(--c-mystic);
                    display: grid;

                    transition: 250ms grid-template-rows ease, 250ms padding ease;

                    &.block__body--hidden {
                        grid-template-rows: 0fr;
                        padding: 0 calc(var(--s) * 8);
                    }

                    &.block__body--open {
                        grid-template-rows: 1fr;
                        padding: calc(var(--s) * 6) calc(var(--s) * 8);
                    }

                    > div {
                        overflow: hidden;
                    }
                }
            }
        `,
    ];
}
