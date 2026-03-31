import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    html,
    customElement,
    property,
    unsafeCSS,
    nothing,
    state,
    query,
    PropertyValues,
} from '@umbraco-cms/backoffice/external/lit';
import { PcbBlockToggleEvent, PcbBlockUpdatedEvent, ON_BLOCK_REMOVE } from '../../../events/block.ts';
import blockHeadStyles from './block-head.css?inline';
import baseStyles from './../../../css/base.css?inline';
import {
    PCBCategoryWithDefinitions,
    PerplexBlockDefinition,
    PerplexContentBlocksBlock,
    Section,
} from '../../../types.ts';
import { PcbValueCopiedEvent } from '../../../events/copyPaste.ts';
import { PcbToastEvent } from '../../../events/toast.ts';
import { store } from '../../../state/store.ts';
import { connect } from 'pwa-helpers';
import { getCategoriesForDefinition } from '../../../utils/block.ts';

const OLD_SYNTAX_SINGLE_VALUE = /^\{\{\s*(\w+)\s*\}\}$/;

@customElement('pcb-block-head')
export default class PcbBlockHead extends connect(store)(UmbLitElement) {
    @property()
    definition?: PerplexBlockDefinition;

    @property({ attribute: false })
    blockDefinitionName!: string;

    @property({ attribute: false })
    get blockNameTemplate(): string | undefined {
        return this._blockNameTemplate;
    }

    set blockNameTemplate(value: string | undefined) {
        if (value != null) {
            const match = value.match(OLD_SYNTAX_SINGLE_VALUE);
            if (match) {
                // For backwards compatibility we will transform the old {{ALIAS}} syntax to the new {umbValue: ALIAS} syntax
                // but only when the entire template was just a single value, e.g. "{{title}}".
                value = `{umbValue: ${match[1]}}`;
            }
        }

        this._blockNameTemplate = value;
    }

    private _blockNameTemplate?: string;

    @property({ attribute: false })
    collapsed: boolean = false;

    @property({ attribute: false })
    id!: string;

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

    @state()
    private blockValuesByAlias: Record<string, any> = {};

    @property({ attribute: false })
    section: Section = Section.CONTENT;

    @property()
    isDraggingBlock: boolean = false;

    @property()
    isMandatory!: boolean;

    @state()
    selectedLayoutIndex: number = 0;

    @state()
    isTouchDevice: boolean = false;

    @state()
    categoryWithDefinitions: PCBCategoryWithDefinitions[] = [];

    @query('#tooltip-popover')
    private _tooltipPopover!: HTMLElement;

    private getIcon() {
        const categories = getCategoriesForDefinition(this.definition?.id ?? '', this.categoryWithDefinitions);
        if (this.definition?.icon) return this.definition.icon;
        if (categories.length > 0) return categories[0].icon;
        return 'icon-block-default';
    }

    #tooltipOnMouseEnter() {
        if (!this.collapsed) {
            this._tooltipPopover.showPopover();
        }
    }

    #tooltipOnMouseLeave() {
        this._tooltipPopover.hidePopover();
    }

    stateChanged(state: any) {
        this.isTouchDevice = state.isTouchDevice;
        this.categoryWithDefinitions = state.definitions.value;
    }

    onHeadClicked = () => {
        this.dispatchEvent(new PcbBlockToggleEvent(this.id));
    };

    onRemoveClicked = (e: Event) => {
        this.dispatchEvent(new CustomEvent(ON_BLOCK_REMOVE, e));
    };

    onToggleVisibilityClicked = () => {
        const updatedBlock = { ...this.block, isDisabled: !this.block.isDisabled };
        if (this.definition) {
            this.dispatchEvent(new PcbBlockUpdatedEvent(updatedBlock, this.definition, this.section));
        }
    };

    onCopyClicked = () => {
        this.dispatchEvent(new PcbValueCopiedEvent([this.block], this.section));
        this.dispatchEvent(
            new PcbToastEvent('positive', {
                headline: 'Copied!',
                message: `${this.definition?.name} copied to clipboard`,
            }),
        );
    };

    protected willUpdate(_changedProperties: PropertyValues<this>) {
        if (_changedProperties.has('definition') || _changedProperties.has('block')) {
            this.selectedLayoutIndex = this.definition?.layouts.findIndex(l => l.id === this.block.layoutId) || 0;
        }

        if (_changedProperties.has('block')) {
            this.blockValuesByAlias = {};

            if (Array.isArray(this.block?.content?.values)) {
                for (const value of this.block.content.values) {
                    this.blockValuesByAlias[value.alias] = value.value;
                }
            }
        }
    }

    render() {
        return html`
            <div class="block-head ${this.block.isDisabled ? 'block-head--disabled' : ''}">
                <button
                    type="button"
                    @click=${this.onHeadClicked}
                    class=${`block-head__toggle ${this.collapsed ? '' : 'block-head--open'}`}
                >
                    ${!this.isTouchDevice
                        ? html`
                            <div
                                id="tooltip-toggle"
                                class="block-head__handle-wrapper"
                                popovertarget="tooltip-popover"
                                @mouseenter=${this.#tooltipOnMouseEnter}
                                @mouseleave=${this.#tooltipOnMouseLeave}
                            >
                                ${this.section === Section.CONTENT
                                    ? html`
                                        <uui-icon
                                            class="block-head__handle icon icon--base"
                                            name="icon-grip"
                                        ></uui-icon>
                                    `
                                    : nothing
                                }
                            </div>
                            <uui-popover-container id="tooltip-popover">
                                <div
                                    style="background-color: var(--uui-color-surface); max-width: 150px; box-shadow: var(--uui-shadow-depth-4); padding: var(--uui-size-space-4); border-radius: var(--uui-border-radius); font-size: 0.9rem;"
                                >
                                    An expanded block cannot be dragged. Collapse the block to drag it.
                                </div>
                            </uui-popover-container>
                        `
                        : nothing
                    }
                    <div class="block-head__title">
                        <strong>
                            <umb-ufm-render
                                inline
                                .markdown=${this.blockNameTemplate}
                                .value=${this.blockValuesByAlias}
                            ></umb-ufm-render>
                        </strong>
                        <div>${this.blockDefinitionName}</div>
                    </div>
                </button>
                ${this.isDraggingBlock
                    ? nothing
                    : html`
                        <pcb-inline-layout-switch
                            .definition=${this.definition}
                            .initialSlideIndex=${this.selectedLayoutIndex}
                        ></pcb-inline-layout-switch>
                    `
                }
                <div class="block-head__controls">
                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onToggleVisibilityClicked}
                    >
                        ${this.block.isDisabled
                            ? html`
                                <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-eye-off-icon lucide-eye-off">
                                    <path d="M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49"/>
                                    <path d="M14.084 14.158a3 3 0 0 1-4.242-4.242"/>
                                    <path d="M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143"/>
                                    <path d="m2 2 20 20"/>
                                </svg>
                            `
                            : html`
                                <uui-icon
                                    style="font-size: 20px;"
                                    name="icon-eye"
                                >
                                </uui-icon>
                            `
                        }
                    </button>

                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onCopyClicked}
                    >
                        <uui-icon
                            style="font-size: 20px; color: var(--c-submarine);"
                            name="icon-documents"
                        >
                        </uui-icon>
                    </button>
                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onRemoveClicked}
                        ?disabled=${this.isMandatory}
                    >
                        <uui-icon
                            style="font-size: 20px; color: var(--c-submarine);"
                            name="icon-trash"
                        >
                        </uui-icon>
                    </button>
                </div>
            </div>
        `;
    }

    static styles = [unsafeCSS(blockHeadStyles), unsafeCSS(baseStyles)];
}
