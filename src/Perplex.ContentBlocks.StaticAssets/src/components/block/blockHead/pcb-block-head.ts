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

const OLD_SYNTAX_SINGLE_VALUE = /^\{\{\s*(\w+)\s*\}\}$/;

@customElement('pcb-block-head')
export default class PcbBlockHead extends connect(store)(UmbLitElement) {
    @property({ attribute: false })
    definition!: PerplexBlockDefinition;

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

    @query('#tooltip-header-block')
    private _tooltipHeaderBlock!: HTMLElement;

    private crossedEyeIcon(fontSize = '20px') {
        return html`
            <uui-icon style="font-size: ${fontSize};">
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="22"
                    height="22"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    class="lucide lucide-eye-off-icon lucide-eye-off"
                >
                    <path
                        d="M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49"
                    />
                    <path d="M14.084 14.158a3 3 0 0 1-4.242-4.242" />
                    <path
                        d="M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143"
                    />
                    <path d="m2 2 20 20" />
                </svg>
            </uui-icon>
        `;
    }

    #tooltipOnMouseEnter() {
        if (!this.collapsed) {
            this._tooltipPopover.showPopover();
        }

        if (this.collapsed && this.section === Section.HEADER) {
            this._tooltipHeaderBlock.showPopover();
        }
    }

    #tooltipOnMouseLeave() {
        this._tooltipPopover.hidePopover();
        this._tooltipHeaderBlock.hidePopover();
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
        this.dispatchEvent(new PcbBlockUpdatedEvent(updatedBlock, this.definition, this.section));
    };

    onCopyClicked = () => {
        this.dispatchEvent(new PcbValueCopiedEvent([this.block], this.section));
        this.dispatchEvent(
            new PcbToastEvent('positive', {
                headline: 'Copied!',
                message: `${this.definition.name} copied to clipboard`,
            }),
        );
    };

    private get hasBlockNameValue(): boolean {
        if (!this.blockNameTemplate) return false;

        // Matches: {prefix: alias}  {=alias}  ${ alias }
        const aliasPattern = /\{(?:[^:}]+:\s*(\w+)|=(\w+))|\$\{\s*(\w+)/g;
        let match;
        let hasAnyAlias = false;

        while ((match = aliasPattern.exec(this.blockNameTemplate)) !== null) {
            hasAnyAlias = true;
            const alias = match[1] ?? match[2] ?? match[3];
            if (alias && this.blockValuesByAlias[alias]) {
                return true;
            }
        }

        // No UFM aliases found — plain text template, always show
        return !hasAnyAlias;
    }

    protected willUpdate(_changedProperties: PropertyValues<this>) {
        if (_changedProperties.has('definition') || _changedProperties.has('block')) {
            this.selectedLayoutIndex = this.definition.layouts.findIndex(l => l.id === this.block.layoutId) || 0;
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
                                  popovertarget=${this.collapsed && this.section === Section.HEADER
                                      ? 'tooltip-header-block'
                                      : 'tooltip-popover'}
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
                                      : nothing}
                              </div>
                              <uui-popover-container id="tooltip-popover">
                                  <div
                                      style="font-size: var(--fs-xs); color: var(--c-white); max-width: 320px; padding: var(--uui-size-space-4); background-color: var(--c-black); border-radius: var(--uui-border-radius); box-shadow: var(--uui-shadow-depth-4);"
                                  >
                                      An expanded block cannot be dragged. Collapse the block to drag it.
                                  </div>
                              </uui-popover-container>
                              ${this.section === Section.HEADER
                                  ? html`
                                        <uui-popover-container id="tooltip-header-block">
                                            <div
                                                style="font-size: var(--fs-xs); color: var(--c-white); max-width: 320px; padding: var(--uui-size-space-4); background-color: var(--c-black); border-radius: var(--uui-border-radius); box-shadow: var(--uui-shadow-depth-4);"
                                            >
                                                A header block cannot be dragged because it should always be positioned
                                                at the top op the page.
                                            </div>
                                        </uui-popover-container>
                                    `
                                  : nothing}
                          `
                        : nothing}
                    <div class="block-head__title">
                        ${this.hasBlockNameValue
                            ? html`<strong>
                                  <umb-ufm-render
                                      inline
                                      .markdown=${this.blockNameTemplate}
                                      .value=${this.blockValuesByAlias}
                                  ></umb-ufm-render>
                              </strong>`
                            : nothing}
                        ${this.block.isDisabled
                            ? html`
                                  <uui-tag style="--uui-tag-border-radius: 30px;">
                                      ${this.crossedEyeIcon('12px')}
                                      <span>Hidden</span>
                                  </uui-tag>
                              `
                            : nothing}
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
                      `}
                <div class="block-head__controls">
                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onToggleVisibilityClicked}
                    >
                        ${this.block.isDisabled
                            ? this.crossedEyeIcon()
                            : html`
                                  <uui-icon
                                      style="font-size: 20px;"
                                      name="icon-eye"
                                  >
                                  </uui-icon>
                              `}
                    </button>

                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onCopyClicked}
                    >
                        <uui-icon
                            style="font-size: 20px;"
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
                            style="font-size: 20px;"
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
