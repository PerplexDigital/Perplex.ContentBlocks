import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, customElement, property, PropertyValues, unsafeCSS } from '@umbraco-cms/backoffice/external/lit';
import styles from './pcb-inline-layout-switch.css?inline';
import { PerplexBlockDefinition } from '../../../types.ts';
import { initSwiper } from '../../../utils/swiper.ts';
import { Swiper } from 'swiper/types';
import { BLockLayoutChangeEvent } from '../../../events/block.ts';
type SwiperContainerEl = HTMLElement & { swiper: Swiper };
@customElement('pcb-inline-layout-switch')
export default class PerplexContentBlocksBlockElement extends UmbLitElement {
    @property()
    definition?: PerplexBlockDefinition;

    @property({ attribute: false })
    initialSlideIndex: number = 0;

    protected firstUpdated(_changedProperties: PropertyValues) {
        const swiperEl = this.shadowRoot?.querySelector<SwiperContainerEl>('swiper-container');
        const swiperPreviewEl = this.shadowRoot?.querySelector<SwiperContainerEl>('swiper-container#swiper-preview');

        if (swiperEl && swiperPreviewEl) {
            initSwiper(swiperEl);
            initSwiper(swiperPreviewEl);
            swiperEl.swiper.slideTo(this.initialSlideIndex, 0, false);

            swiperEl.addEventListener('swiperprogress', ((event: CustomEvent<[Swiper, number]>) => {
                const [swiper, progress] = event.detail;

                // progress is between 0 and 1 → scale it to slide count
                const slideCount = swiper.slides.length;
                const rawIndex = progress * (slideCount - 1);
                const index = Math.round(rawIndex);

                swiperPreviewEl.swiper.slideTo(index, 500, true);

                const selectedLayout = this.definition!.layouts[index];
                this.dispatchEvent(BLockLayoutChangeEvent(selectedLayout));
            }) as EventListener);
        }
    }

    render() {
        if (!this.definition?.layouts) return;
        return html`<div class="inline-layout-switch">
            <swiper-container
                slides-per-view="1"
                speed="500"
                height="100%"
                class="swiper--small"
                allow-touch-move="false"
                navigation="true"
                init="false"
                pagination="false"
            >
                ${this.definition.layouts.map(
                    (layout) => html`
                        <swiper-slide>
                            <div class="inline-layout-switch__layout"><span>${layout.name}</span></div>
                        </swiper-slide>
                    `,
                )}
            </swiper-container>

            <div class="inline-layout-switch__preview">
                <swiper-container
                    slides-per-view="1"
                    speed="0"
                    height="100%"
                    allow-touch-move="false"
                    navigation="false"
                    init="false"
                    pagination="false"
                    id="swiper-preview"
                >
                    ${this.definition!.layouts.map(
                        (layout) => html`
                            <swiper-slide>
                                <div class="inline-layout-switch__layout">
                                    <img
                                        src=${layout.previewImage}
                                        alt="Preview image for ${this.definition!.name}"
                                    />
                                </div>
                            </swiper-slide>
                        `,
                    )}
                </swiper-container>
            </div>
        </div>`;
    }

    static styles = [unsafeCSS(styles)];
}
