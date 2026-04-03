import {
    html,
    customElement,
    property,
    state,
    LitElement,
    PropertyValues,
    unsafeCSS,
    css,
    nothing,
} from '@umbraco-cms/backoffice/external/lit';
import { PerplexBlockDefinition, PerplexContentBlocksBlock } from '../../../types.ts';
import { UUICardElement } from '@umbraco-cms/backoffice/external/uui';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { ON_BLOCK_SELECTED } from '../../../events/block.ts';
import styles from './pcb-block-definition.css?inline';

@customElement('pcb-block-definition')
export class PcbBlockDefinition extends LitElement {
    @property({ attribute: false })
    definition!: PerplexBlockDefinition;

    @property()
    disabled?: boolean;

    @property()
    selectedDefinition?: string;

    @property()
    selectedLayout?: string;

    @property({ type: String, attribute: 'background' })
    background?: string;

    @state()
    selectedLayoutIndex = 0;

    @state()
    selected: boolean = false;

    updated(changedProperties: PropertyValues) {
        if (
            changedProperties.has('selectedDefinition') ||
            changedProperties.has('selectedLayout') ||
            changedProperties.has('definition') ||
            changedProperties.has('selectedLayoutIndex')
        ) {
            this.selected =
                this.definition.id === this.selectedDefinition &&
                this.definition.layouts[this.selectedLayoutIndex].id === this.selectedLayout;
        }
    }

    private _prev = (e: Event) => {
        e.stopPropagation();
        if (this.selectedLayoutIndex > 0) {
            this.selectedLayoutIndex--;
        }
    };

    private _next = (e: Event) => {
        e.stopPropagation();
        if (this.selectedLayoutIndex < this.definition.layouts.length - 1) {
            this.selectedLayoutIndex++;
        }
    };

    private _goToSlide = (index: number, e: Event) => {
        e.stopPropagation();
        this.selectedLayoutIndex = index;
    };

    onSelected() {
        this.dispatchEvent(
            new CustomEvent(ON_BLOCK_SELECTED, {
                bubbles: true,
                detail: {
                    id: UmbId.new(),
                    definitionId: this.definition.id,
                    layoutId: this.definition.layouts[this.selectedLayoutIndex].id,
                    isDisabled: false,
                    content: {
                        key: UmbId.new(),
                        contentTypeKey: this.definition.elementTypeKey,
                    },
                } as PerplexContentBlocksBlock,
                composed: true,
            }),
        );
    }

    render() {
        const layouts = this.definition.layouts;
        const current = layouts[this.selectedLayoutIndex];
        const hasPrev = this.selectedLayoutIndex > 0;
        const hasNext = this.selectedLayoutIndex < layouts.length - 1;
        const hasMultiple = layouts.length > 1;

        return html`
            <div class="blockDefinition ${this.selected ? 'blockDefinition--selected' : ''}">
                <button
                    class="blockDefinition__inner"
                    @click=${this.onSelected}
                    ?disabled=${this.disabled}
                >
                    <div id="portrait">
                        <img
                            src=${current.previewImage}
                            alt="Preview image for ${this.definition.name}"
                            loading="lazy"
                            decoding="async"
                        />
                    </div>

                    <div id="open-part">
                        <strong
                            >${current.name === 'Default'
                                ? this.definition.name
                                : `${this.definition.name} | ${current.name}`}</strong
                        >
                        <span>${this.definition.description}</span>
                    </div>

                    <div class="blockDefinition__controls">
                        ${hasMultiple
                            ? html`
                                  <div class="blockDefinition__nav">
                                      <button
                                          type="button"
                                          class="blockDefinition__nav-btn"
                                          ?disabled=${!hasPrev}
                                          @click=${this._prev}
                                          aria-label="Previous layout"
                                      >
                                          <uui-icon
                                              name="icon-arrow-left"
                                              style="font-size:12px;"
                                          ></uui-icon>
                                      </button>
                                      <div class="blockDefinition__dots">
                                          ${layouts.map(
                                              (_, i) => html`
                                                  <button
                                                      type="button"
                                                      class="blockDefinition__dot ${i === this.selectedLayoutIndex
                                                          ? 'blockDefinition__dot--active'
                                                          : ''}"
                                                      @click=${(e: Event) => this._goToSlide(i, e)}
                                                      aria-label="Layout ${i + 1}"
                                                  ></button>
                                              `,
                                          )}
                                      </div>
                                      <button
                                          type="button"
                                          class="blockDefinition__nav-btn"
                                          ?disabled=${!hasNext}
                                          @click=${this._next}
                                          aria-label="Next layout"
                                      >
                                          <uui-icon
                                              name="icon-arrow-right"
                                              style="font-size:12px;"
                                          ></uui-icon>
                                      </button>
                                  </div>
                              `
                            : nothing}
                    </div>
                </button>
            </div>
        `;
    }

    static styles = [
        ...UUICardElement.styles,
        unsafeCSS(styles),
        css`
            .blockDefinition__nav {
                display: flex;
                align-items: center;
                justify-content: center;
                gap: 4px;
                padding: 2px 0;
            }

            .blockDefinition__nav-btn {
                display: flex;
                align-items: center;
                justify-content: center;
                width: 24px;
                height: 24px;
                border: none;
                background: transparent;
                cursor: pointer;
                color: inherit;
                padding: 0;
                border-radius: var(--uui-border-radius);
            }

            .blockDefinition__nav-btn:hover:not(:disabled) {
                background-color: var(--uui-color-surface-alt);
            }

            .blockDefinition__nav-btn:disabled {
                opacity: 0.3;
                cursor: default;
            }

            .blockDefinition__dots {
                display: flex;
                align-items: center;
                gap: 4px;
            }

            .blockDefinition__dot {
                width: 8px;
                height: 8px;
                border-radius: 50%;
                border: 1px solid var(--uui-color-border);
                background: var(--uui-color-surface);
                padding: 0;
                cursor: pointer;
                transition: background-color 150ms ease;
            }

            .blockDefinition__dot--active {
                background: var(--uui-color-interactive);
                border-color: var(--uui-color-interactive);
            }

            .blockDefinition__dot:hover:not(.blockDefinition__dot--active) {
                background: var(--uui-color-surface-alt);
            }
        `,
    ];
}
