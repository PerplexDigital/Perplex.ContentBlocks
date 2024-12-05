import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, customElement, state, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import { UMB_AUTH_CONTEXT, UmbAuthContext } from '@umbraco-cms/backoffice/auth';

@customElement('pcb-preview')
export default class PerplexContentBlocksPreviewElement extends UmbLitElement {
    #authContext!: UmbAuthContext;

    @state()
    private ok: boolean = false;

    @state()
    private token?: string;

    @state()
    private get previewUrl(): string {
        const qs = [];

        qs.push(`pageId=${this.pageId}`);
        if (this.culture != null) qs.push(`culture=${this.culture}`);
        qs.push(`token=${this.token}`);

        return `/umbraco/perplex-content-blocks/api/preview?${qs.join('&')}`;
    }

    @property({ attribute: false })
    pageId!: string;

    @property({ attribute: false })
    culture!: string | null;

    constructor() {
        super();

        this.consumeContext(UMB_AUTH_CONTEXT, (ctx) => {
            this.#authContext = ctx;
        });
    }

    async firstUpdated() {
        const token = await this.#authContext.getLatestToken();
        if (token != null && token.length > 0) {
            this.token = token;
            this.ok = true;
        }
    }

    scrollToBlock(blockId: string) {
        const iframe = this.renderRoot.querySelector('iframe');
        if (iframe == null) {
            // Cannot scroll without an iframe
            return;
        }

        iframe.contentWindow?.postMessage({ blockId });
    }

    render() {
        if (!this.ok) return nothing;

        return html`<div style="position:relative">
            <iframe
                src=${this.previewUrl}
                style="transform:scale(0.234375) translateZ(0px);width:1280px;height:755px;position:absolute;top:0;left:0;transform-origin:top left"
            ></iframe>
        </div>`;
    }
}
