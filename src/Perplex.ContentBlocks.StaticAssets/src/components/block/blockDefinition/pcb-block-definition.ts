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
    selected?: boolean;

    @property({ type: String, attribute: 'background' })
    background?: string;

    @state()
    selectedLayoutIndex = 0;

    connectedCallback() {
        super.connectedCallback();
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
                const [, progress] = event.detail;
                this.selectedLayoutIndex = progress;
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
                class="blockDefinition"
                navigation="true"
                init="false"
                pagination="true"
            >
                ${this.definition!.layouts.map(
                    (layout) => html`
                        <swiper-slide>
                            <button
                                class="blockDefinition"
                                @click=${this.onSelected}
                            >
                                <div id="portrait">
                                    <img
                                        src=${layout.previewImage}
                                        alt="Preview image for ${this.definition!.name}"
                                    />
                                </div>

                                <button
                                    id="open-part"
                                    tabindex=${this.disabled ? (nothing as any) : '0'}
                                >
                                    <strong
                                        >${layout.name === 'Default'
                                            ? this.definition!.name
                                            : `${this.definition!.name} | ${layout.name}`}</strong
                                    >
                                    <span>${this.definition!.description}</span>
                                </button>

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
