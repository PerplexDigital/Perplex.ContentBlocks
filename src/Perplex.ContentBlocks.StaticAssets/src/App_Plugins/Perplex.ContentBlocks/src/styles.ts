import {css} from "@umbraco-cms/backoffice/external/lit";

export const variables = css`
    :host {
        --c-submarine: 190, 190, 190;
        --c-mystic: #f6f6f6;
        --c-wild-sand: #f5f5f5;
        --c-black: #212121;

        --s: 4px;

        --fs-xs: calc(var(--s) * 3);
        --fs-sm: calc(var(--s) * 3.5);
        --fs-base: calc(var(--s) * 4);
        --fs-lg: calc(var(--s) * 4.5);
        --fs-xl: calc(var(--s) * 5);
        --fs-2xl: calc(var(--s) * 6);
        --fs-3xl: calc(var(--s) * 7.5);
        --fs-4xl: calc(var(--s) * 9);
        --fs-5xl: calc(var(--s) * 12);
        --fs-6xl: calc(var(--s) * 15);
        --fs-7xl: calc(var(--s) * 18);
        --fs-8xl: calc(var(--s) * 24);
        --fs-9xl: calc(var(--s) * 32);

        --r-base: 2px;
        --r-lg: 4px;

        --bs-base: var(--uui-shadow-depth-1, 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24));

    }
`;

