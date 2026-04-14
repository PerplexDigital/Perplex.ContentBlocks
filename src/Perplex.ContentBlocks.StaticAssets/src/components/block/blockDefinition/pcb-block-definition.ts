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

// Swiper-style chevron SVG (pointing right). Mirrored via CSS transform for "prev".
const chevronSvg = html`<svg
    width="11"
    height="20"
    viewBox="0 0 11 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
>
    <path
        d="M0.38296 20.0762C0.111788 19.805 0.111788 19.3654 0.38296 19.0942L9.19758 10.2796L0.38296 1.46497C0.111788 1.19379 0.111788 0.754138 0.38296 0.482966C0.654131 0.211794 1.09379 0.211794 1.36496 0.482966L10.4341 9.55214C10.8359 9.9539 10.8359 10.6053 10.4341 11.007L1.36496 20.0762C1.09379 20.3474 0.654131 20.3474 0.38296 20.0762Z"
        fill="currentColor"
    ></path>
</svg>`;

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
        const currentNumber = this.selectedLayoutIndex + 1;
        const hasMultiple = layouts.length > 1;

        return html`
            <div class="blockDefinition ${this.selected ? 'blockDefinition--selected' : ''}">
                <button
                    class="blockDefinition__inner"
                    @click=${this.onSelected}
                    ?disabled=${this.disabled}
                >
                    <div id="portrait">
                        <div class="portrait__placeholder">
                            <img
                                src=${current.previewImage}
                                alt="Preview image for ${this.definition.name}"
                                loading="lazy"
                                decoding="async"
                            />
                        </div>

                        ${hasMultiple
                            ? html`
                                  <button
                                      type="button"
                                      class="blockDefinition__nav-btn blockDefinition__nav-btn--prev"
                                      ?disabled=${!hasPrev}
                                      @click=${this._prev}
                                      aria-label="Previous layout"
                                  >
                                      ${chevronSvg}
                                  </button>
                                  <button
                                      type="button"
                                      class="blockDefinition__nav-btn blockDefinition__nav-btn--next"
                                      ?disabled=${!hasNext}
                                      @click=${this._next}
                                      aria-label="Next layout"
                                  >
                                      ${chevronSvg}
                                  </button>
                              `
                            : nothing}
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
                        <div class="blockDefinition__control-wrapper">
                            <strong>${current.name}</strong>
                            ${hasMultiple ? html` <uui-tag>${currentNumber}/${layouts.length}</uui-tag> ` : nothing}
                        </div>
                    </div>
                </button>
            </div>
        `;
    }

    static styles = [
        ...UUICardElement.styles,
        unsafeCSS(styles),
        css`
            /* Prev/next arrow buttons overlaid on the portrait area */
            .blockDefinition__nav-btn {
                position: absolute;
                top: 50%;
                transform: translateY(-50%);
                z-index: 10;
                display: flex;
                align-items: center;
                justify-content: center;
                width: 2rem;
                height: 2rem;
                border: none;
                border-radius: 100%;
                background-color: var(--uui-palette-mine-grey);
                cursor: pointer;
                color: white;
                padding: 0;
            }

            .blockDefinition__nav-btn:focus {
                border-radius: 100%;
            }

            .blockDefinition__nav-btn svg {
                width: 0.5rem;
            }

            .blockDefinition__nav-btn--prev {
                left: 10px;
            }

            .blockDefinition__nav-btn--prev svg {
                transform: rotate(180deg);
            }

            .blockDefinition__nav-btn--next {
                right: 10px;
            }

            .blockDefinition__nav-btn:hover:not(:disabled) {
                opacity: 0.8;
            }

            .blockDefinition__nav-btn:disabled {
                opacity: 0.35;
                cursor: default;
            }

            /* Pagination dots in the controls area */
            .blockDefinition__pagination {
                display: flex;
                align-items: center;
                justify-content: center;
                gap: 4px;
                height: 100%;
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
