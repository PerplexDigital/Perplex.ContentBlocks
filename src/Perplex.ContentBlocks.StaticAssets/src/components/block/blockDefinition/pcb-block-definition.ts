import {
    html,
    customElement,
    property,
    nothing,
    state,
    LitElement,
    PropertyValues,
    unsafeCSS,
} from '@umbraco-cms/backoffice/external/lit';
import { PerplexBlockDefinition, PerplexContentBlocksBlock } from '../../../types.ts';
import { UUICardElement } from '@umbraco-cms/backoffice/external/uui';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { ON_BLOCK_SELECTED } from '../../../events/block.ts';
import { initSwiper } from '../../../utils/swiper.ts';
import styles from './pcb-block-definition.css?inline';
import { Swiper } from 'swiper/types';

@customElement('pcb-block-definition')
export class PcbBlockDefinition extends LitElement {
    @property()
    definition?: PerplexBlockDefinition;

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

    connectedCallback() {
        super.connectedCallback();
    }

    updated(changedProperties: PropertyValues) {
        if (
            changedProperties.has('selectedDefinition') ||
            changedProperties.has('selectedLayout') ||
            changedProperties.has('definition') ||
            changedProperties.has('selectedLayoutIndex')
        ) {
            if (!this.definition) {
                return;
            }

            this.selected =
                this.definition.id === this.selectedDefinition &&
                this.definition.layouts[this.selectedLayoutIndex].id === this.selectedLayout;
        }
    }

    onSelected() {
        if (!this.definition) return;

        this.dispatchEvent(
            new CustomEvent(ON_BLOCK_SELECTED, {
                bubbles: true,
                detail: {
                    id: UmbId.new(),
                    definitionId: this.definition.id,
                    layoutId: this.definition?.layouts[this.selectedLayoutIndex].id,
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

    protected firstUpdated(_changedProperties: PropertyValues) {
        const swiperEl = this.shadowRoot?.querySelector<HTMLElement & EventTarget>('swiper-container');

        if (swiperEl) {
            initSwiper(swiperEl);

            swiperEl.addEventListener('swiperprogress', ((event: CustomEvent<[Swiper, number]>) => {
                const [swiper, progress] = event.detail;
                const slideCount = swiper.slides.length;
                const rawIndex = progress * (slideCount - 1);
                const index = Math.round(rawIndex);
                this.selectedLayoutIndex = index;
            }) as EventListener);
        }
    }

    render() {
        if (!this.definition) return html``;

        return html`
            <swiper-container
                slides-per-view="1"
                speed="500"
                allow-touch-move="false"
                class="blockDefinition ${this.selected ? 'blockDefinition--selected' : ''}"
                navigation="true"
                init="false"
                pagination="true"
            >
                ${this.definition!.layouts.map(
                    layout => html`
                        <swiper-slide>
                            <button
                                class="blockDefinition__inner"
                                @click=${this.onSelected}
                            >
                                <div id="portrait">
                                    <img
                                        src=${layout.previewImage}
                                        alt="Preview image for ${this.definition!.name}"
                                    />
                                </div>

                                <div
                                    id="open-part"
                                    tabindex=${this.disabled ? (nothing as any) : '0'}
                                >
                                    <strong
                                        >${layout.name === 'Default'
                                            ? this.definition!.name
                                            : `${this.definition!.name} | ${layout.name}`}</strong
                                    >
                                    <span>${this.definition!.description}</span>
                                </div>

                                <div class="blockDefinition__controls"></div>
                            </button>
                        </swiper-slide>
                    `,
                )}
            </swiper-container>
        `;
    }

    static styles = [...UUICardElement.styles, unsafeCSS(styles)];
}
