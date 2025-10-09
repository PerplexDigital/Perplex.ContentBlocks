import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    classMap,
    css,
    customElement,
    html,
    property,
    repeat,
    state,
    unsafeCSS,
} from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyTypeModel } from '@umbraco-cms/backoffice/content-type';
import { PerplexContentBlocksPropertyDatasetContext } from '../../perplex-content-blocks-dataset-context.ts';
import { UmbDataTypeDetailModel, UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbDocumentTypeDetailModel, UmbDocumentTypeDetailRepository } from '@umbraco-cms/backoffice/document-type';
import { UMB_VALIDATION_CONTEXT, UmbValidationController } from '@umbraco-cms/backoffice/validation';
import contentBlockName from '../../utils/contentBlockName.ts';
import { PerplexBlockDefinition, PerplexContentBlocksBlock, Section } from '../../types.ts';
import { BlockUpdatedEvent, ON_BLOCK_LAYOUT_CHANGE, ON_BLOCK_REMOVE } from '../../events/block.ts';
import { animate } from '@lit-labs/motion';
import { PropertyValues } from 'lit';

import baseStyles from './../../css/base.css?inline';

export function propertyAliasPrefix(block: PerplexContentBlocksBlock): string {
    return block.id + '_';
}

@customElement('pcb-block')
export default class PerplexContentBlocksBlockElement extends UmbLitElement {
    @property({ attribute: false })
    collapsed: boolean = true;

    @property()
    definition?: PerplexBlockDefinition;

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

    @property({ attribute: false })
    removeBlock!: (udi: string) => void;

    @property({ attribute: false })
    dataPath!: string;

    @property({ attribute: false })
    section: Section = Section.CONTENT;

    @state()
    private ok: boolean = false;

    @state()
    properties: UmbPropertyTypeModel[] | undefined = undefined;

    @state()
    removing: boolean = false;

    #contentTypeRepository = new UmbDocumentTypeDetailRepository(this);
    elementType!: UmbDocumentTypeDetailModel;

    #dataTypeRepository = new UmbDataTypeDetailRepository(this);

    #dataTypes: {
        [key: string]: UmbDataTypeDetailModel;
    } = {};

    #validationController?: UmbValidationController;

    connectedCallback() {
        super.connectedCallback();
        const errors: string[] = [];

        if (!this.block) {
            errors.push('block property is required');
        }

        if (!this.removeBlock) {
            errors.push('removeBlock property is required');
        }

        if (!this.removeBlock) {
            errors.push('onChange property is required');
        }

        if (errors.length > 0) {
            throw new Error(errors.join(' | '));
        }

        this.addEventListener(ON_BLOCK_LAYOUT_CHANGE, (e: Event) => this.onLayoutChange(e as CustomEvent));
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.removeEventListener(ON_BLOCK_REMOVE, this.onBlockRemoveClick);
        this.clearValidationMessages();
    }

    onLayoutChange = (event: CustomEvent<any>) => {
        // do nothing if the layout didn't change
        if (this.block.layoutId === event.detail.selectedLayout.id) {
            return;
        }

        const updatedBlock: PerplexContentBlocksBlock = {
            ...this.block,
            layoutId: event.detail.selectedLayout.id,
        };

        if (this.definition) {
            this.dispatchEvent(BlockUpdatedEvent(updatedBlock, this.definition, this.section));
        }
    };

    constructor() {
        super();
        this.consumeContext(UMB_VALIDATION_CONTEXT, (ctx) => {
            this.#validationController = ctx;
        });

        this.addEventListener(ON_BLOCK_REMOVE, this.onBlockRemoveClick);
    }

    onBlockRemoveClick = () => {
        this.removing = true;

        // Remove the block after the animation has finished
        setTimeout(() => {
            this.removeBlock(this.block.id);
        }, 250);
    };

    onBlockUpdate = (block: PerplexContentBlocksBlock) => {
        if (this.definition) {
            this.dispatchEvent(BlockUpdatedEvent(block, this.definition, this.section));
        }
    };

    async firstUpdated() {
        const elementTypeResponse = await this.#contentTypeRepository.requestByUnique(
            this.block.content.contentTypeKey,
        );
        if (elementTypeResponse.data == null) {
            throw new Error(`Cannot retrieve content type ${this.block.content.contentTypeKey}`);
        }

        this.elementType = elementTypeResponse.data;
        new PerplexContentBlocksPropertyDatasetContext(this, this.block, this.onBlockUpdate);

        // TODO: Fetch only the distinct dataTypes + in parallel
        for (const property of this.elementType.properties) {
            const dataTypeResponse = await this.#dataTypeRepository.requestByUnique(property.dataType.unique);
            if (dataTypeResponse.data == null) {
                throw new Error(`Cannot retrieve data type ${property.dataType.unique}`);
            }

            this.#dataTypes[property.dataType.unique] = dataTypeResponse.data;
        }

        this.ok = true;
    }

    clearValidationMessages() {
        for (const property of this.elementType.properties) {
            const path = `${this.dataPath}.${this.block.id}.${property.alias}`;
            this.#validationController?.messages.removeMessagesByTypeAndPath('client', path);
            this.#validationController?.messages.removeMessagesByTypeAndPath('server', path);
        }
    }

    render() {
        if (!this.ok) {
            return;
        }

        const classes = {
            block: true,
            block__removing: this.removing,
        };

        return html`<div
            class="${classMap(classes)}"
            ${animate({
                id: this.block.content.key,
            })}
        >
            <pcb-block-head
                .block=${this.block}
                .id=${this.block.id}
                .blockDefinitionName=${this.definition?.name}
                .collapsed="${this.collapsed}"
                .blockTemplateName="${contentBlockName(this.definition?.blockNameTemplate ?? '', this.block)}"
                .definition=${this.definition}
                .section=${this.section}
            >
            </pcb-block-head>
            <div class=${this.collapsed ? 'block__body block__body--hidden' : 'block__body block__body--open'}>
                <div>
                    ${repeat(
                        this.elementType.properties,
                        (property) => property.id,
                        (property) => {
                            const dataType = this.#dataTypes[property.dataType.unique];
                            if (dataType == null) throw new Error('missing data type');

                            return html`<umb-property
                                .dataPath=${this.dataPath}
                                .alias=${propertyAliasPrefix(this.block) + property.alias}
                                .label=${property.name}
                                .description=${property.description}
                                .appearance=${property.appearance}
                                property-editor-ui-alias=${dataType.editorUiAlias}
                                orientation="vertical"
                                .config=${dataType.values}
                                .validation=${property.validation}
                            >
                            </umb-property>`;
                        },
                    )}
                </div>
            </div>
        </div>`;
    }

    updated(changedProperties: PropertyValues) {
        super.updated(changedProperties);

        this.updateComplete.then(() => {
            Array.from(this.renderRoot.querySelectorAll('umb-property')).forEach((umbProp: any) => {
                const layout = umbProp?.shadowRoot?.querySelector('umb-property-layout');
                if (layout) layout.orientation = 'vertical';
            });
        });
    }

    static styles = [
        unsafeCSS(baseStyles),
        css`
            .block {
                box-shadow: var(--bs-base);

                &.block__removing {
                    background-color: red !important;
                    opacity: 0;
                    transform: scaleY(0);
                    transition:
                        opacity 250ms,
                        transform 250ms;
                }

                .block__body {
                    background-color: var(--c-mystic);
                    display: grid;

                    transition:
                        250ms grid-template-rows ease,
                        250ms padding ease;

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
