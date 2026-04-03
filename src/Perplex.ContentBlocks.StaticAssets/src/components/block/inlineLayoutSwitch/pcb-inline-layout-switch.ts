import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, customElement, property, state, unsafeCSS, nothing } from '@umbraco-cms/backoffice/external/lit';
import styles from './pcb-inline-layout-switch.css?inline';
import { PerplexBlockDefinition } from '../../../types.ts';
import { PcbBlockLayoutChangeEvent } from '../../../events/block.ts';

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

/**
 * Inline layout switcher component that allows users to switch between
 * different layouts for a content block using simple prev/next buttons
 * styled to match the original Swiper navigation arrows.
 */
@customElement('pcb-inline-layout-switch')
export default class PerplexContentBlocksInlineLayoutSwitchElement extends UmbLitElement {
    @property({ attribute: false })
    definition!: PerplexBlockDefinition;

    @property({ attribute: false })
    initialSlideIndex: number = 0;

    @state()
    private currentIndex: number = 0;

    @state()
    private previewLoaded: boolean = false;

    connectedCallback() {
        super.connectedCallback();
        this.currentIndex = this.initialSlideIndex;
    }

    private _loadPreviewImages = () => {
        if (!this.previewLoaded) {
            this.previewLoaded = true;
        }
    };

    private _prev = () => {
        if (this.currentIndex <= 0) return;
        this.currentIndex--;
        this._emitChange();
    };

    private _next = () => {
        if (this.currentIndex >= this.definition.layouts.length - 1) return;
        this.currentIndex++;
        this._emitChange();
    };

    private _emitChange() {
        const selectedLayout = this.definition.layouts[this.currentIndex];
        this.dispatchEvent(new PcbBlockLayoutChangeEvent(selectedLayout));
    }

    render() {
        const layouts = this.definition.layouts;
        if (layouts.length <= 1) {
            // Single layout — just show the name, no navigation
            return layouts.length === 1
                ? html`<div
                      class="inline-layout-switch"
                      @mouseenter=${this._loadPreviewImages}
                      @focusin=${this._loadPreviewImages}
                      @touchstart=${this._loadPreviewImages}
                  >
                      <div class="inline-layout-switch__single">
                          <div class="inline-layout-switch__layout"><span>${layouts[0].name}</span></div>
                      </div>
                      ${this._renderPreview()}
                  </div>`
                : nothing;
        }

        const hasPrev = this.currentIndex > 0;
        const hasNext = this.currentIndex < layouts.length - 1;
        const current = layouts[this.currentIndex];

        return html`<div
            class="inline-layout-switch"
            @mouseenter=${this._loadPreviewImages}
            @focusin=${this._loadPreviewImages}
            @touchstart=${this._loadPreviewImages}
        >
            <div class="inline-layout-switch__nav">
                <button
                    type="button"
                    class="inline-layout-switch__btn inline-layout-switch__btn--prev"
                    ?disabled=${!hasPrev}
                    @click=${this._prev}
                    aria-label="Previous layout"
                >
                    ${chevronSvg}
                </button>
                <div class="inline-layout-switch__layout"><span>${current.name}</span></div>
                <button
                    type="button"
                    class="inline-layout-switch__btn inline-layout-switch__btn--next"
                    ?disabled=${!hasNext}
                    @click=${this._next}
                    aria-label="Next layout"
                >
                    ${chevronSvg}
                </button>
            </div>
            ${this._renderPreview()}
        </div>`;
    }

    private _renderPreview() {
        const current = this.definition.layouts[this.currentIndex];
        if (!current) return nothing;

        return html`
            <div class="inline-layout-switch__preview">
                <div class="inline-layout-switch__layout">
                    ${this.previewLoaded
                        ? html`<img
                              src=${current.previewImage}
                              alt="Preview image for ${this.definition.name}"
                              loading="lazy"
                              decoding="async"
                          />`
                        : nothing}
                </div>
            </div>
        `;
    }

    static styles = [unsafeCSS(styles)];
}
