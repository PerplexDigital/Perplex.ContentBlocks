import { register } from 'swiper/element/bundle';
import swiperCss from './../css/swiper.css?raw';

let registered = false;

export function initSwiper(swiperEl: HTMLElement) {
    if (!registered) {
        register();
        registered = true;
    }
    if (!swiperEl) return;
    const params = {
        injectStyles: [swiperCss],
    };

    Object.assign(swiperEl, params);

    // Only call initialize if it hasn't already been initialized
    if ((swiperEl as any).initialize) {
        (swiperEl as any).initialize();
    }

    return swiperEl;
}
