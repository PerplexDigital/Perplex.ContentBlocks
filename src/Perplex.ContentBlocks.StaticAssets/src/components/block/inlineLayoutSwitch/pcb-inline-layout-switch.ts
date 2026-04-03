import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, customElement, property, state, unsafeCSS, nothing } from '@umbraco-cms/backoffice/external/lit';
import styles from './pcb-inline-layout-switch.css?inline';
import { PerplexBlockDefinition } from '../../../types.ts';
import { PcbBlockLayoutChangeEvent } from '../../../events/block.ts';

/**
 * Inline layout switcher component that allows users to switch between
 * different layouts for a content block using simple prev/next buttons.
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
                    class="inline-layout-switch__btn"
                    ?disabled=${!hasPrev}
                    @click=${this._prev}
                    aria-label="Previous layout"
                >
                    <uui-icon
                        name="icon-arrow-left"
                        style="font-size:12px;"
                    ></uui-icon>
                </button>
                <div class="inline-layout-switch__layout"><span>${current.name}</span></div>
                <button
                    type="button"
                    class="inline-layout-switch__btn"
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

    static styles = [
        unsafeCSS(styles),
        // Additional styles for the button-based nav
        // We keep the existing CSS from the imported stylesheet and add minimal overrides
    ];
}
