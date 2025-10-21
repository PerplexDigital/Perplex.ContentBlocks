import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, customElement, property, unsafeCSS, nothing, state } from '@umbraco-cms/backoffice/external/lit';
import { BlockToggleEvent, BlockUpdatedEvent, ON_BLOCK_REMOVE } from '../../../events/block.ts';
import blockHeadStyles from './block-head.css?inline';
import baseStyles from './../../../css/base.css?inline';
import { PerplexBlockDefinition, PerplexContentBlocksBlock, Section } from '../../../types.ts';
import { PropertyValues } from 'lit';
import { ValueCopiedEvent } from '../../../events/copyPaste.ts';
import { ToastEvent } from '../../../events/toast.ts';
import { store } from '../../../state/store.ts';
import { connect } from 'pwa-helpers';

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

    @state()
    selectedLayoutIndex: number = 0;

    @state()
    isTouchDevice: boolean = false;

    stateChanged(state: any) {
        this.isTouchDevice = state.isTouchDevice;
    }

    onHeadClicked = () => {
        this.dispatchEvent(BlockToggleEvent(this.id));
    };

    onRemoveClicked = (e: Event) => {
        this.dispatchEvent(new CustomEvent(ON_BLOCK_REMOVE, e));
    };

    onToggleVisibilityClicked = () => {
        const updatedBlock = { ...this.block, isDisabled: !this.block.isDisabled };
        if (this.definition) {
            this.dispatchEvent(BlockUpdatedEvent(updatedBlock, this.definition, this.section));
        }
    };

    onCopyClicked = () => {
        this.dispatchEvent(ValueCopiedEvent([this.block]));
        this.dispatchEvent(
            ToastEvent('positive', {
                headline: 'Copied!',
                message: `${this.definition?.name} copied to clipboard`,
            }),
        );
    };

    protected willUpdate(_changedProperties: PropertyValues) {
        if (_changedProperties.has('definition') || _changedProperties.has('block')) {
            this.selectedLayoutIndex = this.definition?.layouts.findIndex((l) => l.id === this.block.layoutId) || 0;
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
                              <uui-icon
                                  class="block-head__handle icon icon--base"
                                  name="icon-grip"
                              >
                              </uui-icon>
                          `
                        : nothing}
                    <div class="block-head__title">
                        ${this.blockTemplateName ? html`<strong>${this.blockTemplateName}</strong>` : nothing}
                        <div>${this.blockDefinitionName}</div>
                    </div>

                    <svg class="block-head__icon icon icon--base">
                        <use href="${this.definition?.icon}"></use>
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
                        class="block-head__control"
                        type="button"
                        @click=${this.onRemoveClicked}
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
