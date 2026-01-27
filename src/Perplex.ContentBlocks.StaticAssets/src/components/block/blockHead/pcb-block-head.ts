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

@customElement('pcb-block-head')
export default class PcbBlockHead extends connect(store)(UmbLitElement) {
    @property()
    definition?: PerplexBlockDefinition;

    @property({ attribute: false })
    blockDefinitionName!: string;

    @property({ attribute: false })
    blockTemplateName?: string;

    @property({ attribute: false })
    collapsed: boolean = false;

    @property({ attribute: false })
    id!: string;

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

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

    protected willUpdate(_changedProperties: PropertyValues) {
        if (_changedProperties.has('definition') || _changedProperties.has('block')) {
            this.selectedLayoutIndex = this.definition?.layouts.findIndex(l => l.id === this.block.layoutId) || 0;
        }
    }

    render() {
        return html`
            <div class="block-head">
                <button
                    type="button"
                    @click=${this.onHeadClicked}
                    class=${`block-head__toggle ${this.block.isDisabled ? 'block-head__toggle--disabled' : ''} ${this.collapsed ? '' : 'block-head--open'}`}
                >
                    ${this.section === Section.CONTENT && !this.isTouchDevice
                        ? html`
                              <b
                                  id="tooltip-toggle"
                                  popovertarget="tooltip-popover"
                                  @mouseenter=${this.#tooltipOnMouseEnter}
                                  @mouseleave=${this.#tooltipOnMouseLeave}
                              >
                                  <uui-icon
                                      class="block-head__handle icon icon--base"
                                      name="icon-grip"
                                  >
                                  </uui-icon
                              ></b>
                              <uui-popover-container id="tooltip-popover">
                                  <div
                                      style="background-color: var(--uui-color-surface); max-width: 150px; box-shadow: var(--uui-shadow-depth-4); padding: var(--uui-size-space-4); border-radius: var(--uui-border-radius); font-size: 0.9rem;"
                                  >
                                      An expanded block cannot be dragged. Collapse the block to drag it.
                                  </div>
                              </uui-popover-container>
                          `
                        : nothing}
                    <div class="block-head__title">
                        ${this.blockTemplateName ? html`<strong>${this.blockTemplateName}</strong>` : nothing}
                        <div>${this.blockDefinitionName}</div>
                    </div>

                    <svg class="block-head__icon icon icon--base">
                        <use href="${this.getIcon()}"></use>
                    </svg>
                </button>
                ${this.isDraggingBlock
                    ? nothing
                    : html`
                          <pcb-inline-layout-switch
                              .definition=${this.definition}
                              .initialSlideIndex=${this.selectedLayoutIndex}
                          ></pcb-inline-layout-switch>
                      `}

                <div class="block-head__controls">
                    <button
                        class="block-head__control ${this.block.isDisabled ? 'block-head__control--disabled' : ''}"
                        type="button"
                        @click=${this.onToggleVisibilityClicked}
                    >
                        <uui-icon
                            style="font-size: 20px; color: var(--c-submarine);"
                            name="icon-eye"
                        >
                        </uui-icon>
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
                        class="block-head__control ${this.isMandatory ? 'block-head__control--disabled' : ''}"
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
