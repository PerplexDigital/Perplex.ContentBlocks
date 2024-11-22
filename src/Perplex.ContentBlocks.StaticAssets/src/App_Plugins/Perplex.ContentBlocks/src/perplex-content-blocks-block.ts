import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { html, repeat, customElement, property, css, state } from "@umbraco-cms/backoffice/external/lit";
import type { UmbPropertyTypeModel } from "@umbraco-cms/backoffice/content-type";
import { PerplexContentBlocksPropertyDatasetContext } from "./perplex-content-blocks-dataset-context";
import { PerplexContentBlocksBlock, PerplexContentBlocksBlockOnChangeFn } from "./perplex-content-blocks";
import { UmbDocumentTypeDetailModel, UmbDocumentTypeDetailRepository } from "@umbraco-cms/backoffice/document-type";
import { UMB_VALIDATION_CONTEXT, UmbValidationController } from "@umbraco-cms/backoffice/validation";

@customElement("perplex-content-blocks-block")
export default class PerplexContentBlocksBlockElement extends UmbLitElement {
    @state()
    private ok: boolean = false;

    #contentTypeRepository = new UmbDocumentTypeDetailRepository(this);
    elementType!: UmbDocumentTypeDetailModel;

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

    #validationContext!: UmbValidationController;

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
            <div class="block-body">
                ${repeat(
                    this.elementType.properties,
                    property => property.id,
                    property =>
                        html`<umb-property-type-based-property
                            .property=${property}
                            .dataPath=${`${this.dataPath}.${this.block.id}.${property.alias}`}
                        ></umb-property-type-based-property>`
                )}
            </div>
            <button type="button" class="remove-block" @click=${this.removeBlock.bind(null, this.block.content.key)}>
                &times;
            </button>
        </div>`;
    }

    static styles = [
        css`
            .block {
                display: flex;
                border: 3px solid #2e3436;
                min-height: 4rem;

                .block-body {
                    flex: 1;
                    padding: 0.5rem 1rem;
                }

                .remove-block {
                    font-size: 2rem;
                    width: 2rem;
                    height: 2rem;
                    color: lightgray;
                    border: none;
                    background: none;

                    &:hover {
                        cursor: pointer;
                        color: #cc0000;
                    }
                }
            }
        `,
    ];
}
