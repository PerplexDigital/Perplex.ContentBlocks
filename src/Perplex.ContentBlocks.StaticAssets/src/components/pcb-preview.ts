import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    html,
    customElement,
    state,
    property,
    css,
    nothing,
    PropertyValues,
    query,
} from '@umbraco-cms/backoffice/external/lit';
import { UMB_AUTH_CONTEXT, UmbAuthContext } from '@umbraco-cms/backoffice/auth';
import { UMB_ACTION_EVENT_CONTEXT } from '@umbraco-cms/backoffice/action';
import { UmbRequestReloadStructureForEntityEvent } from '@umbraco-cms/backoffice/entity-action';
import { UMB_DOCUMENT_ENTITY_TYPE } from '@umbraco-cms/backoffice/document';

@customElement('pcb-preview')
export default class PerplexContentBlocksPreviewElement extends UmbLitElement {
    #authContext?: UmbAuthContext;
    #resizeObserver?: ResizeObserver;

    @state()
    private ok = false;

    @state()
    private token?: string;

    @state()
    private previewMode: 'desktop' | 'mobile' = 'desktop';

    @state()
    private lastUpdate?: Date;

    @property({ attribute: false })
    pageId!: string;

    @property({ attribute: false })
    culture!: string | null;

    @property()
    focusedBlockId?: string;

    @query('.preview-frame')
    private iframe?: HTMLIFrameElement;

    @query('.iframe-frame')
    private iframeFrame?: HTMLElement;

    private _lastFocusedBlock?: string;

    private readonly DESKTOP_WIDTH = 1280;
    private readonly DESKTOP_HEIGHT = 800;
    private readonly MOBILE_WIDTH = 345;
    private readonly MOBILE_HEIGHT = 475;

    get previewUrl(): string {
        const qs = [`pageId=${this.pageId}`];
        if (this.culture != null) qs.push(`culture=${this.culture}`);
        if (this.token) qs.push(`token=${this.token}`);
        return `/umbraco/perplex-content-blocks/api/preview?${qs.join('&')}`;
    }

    private get iframeWidth(): number {
        return this.previewMode === 'desktop' ? this.DESKTOP_WIDTH : this.MOBILE_WIDTH;
    }

    private get iframeHeight(): number {
        return this.previewMode === 'desktop' ? this.DESKTOP_HEIGHT : this.MOBILE_HEIGHT;
    }

    private get formattedLastUpdate(): string {
        if (!this.lastUpdate) return '';
        const date = this.lastUpdate;
        const pad = (n: number) => n.toString().padStart(2, '0');
        return `${pad(date.getDate())}/${pad(date.getMonth() + 1)}/${date.getFullYear()} ${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
    }

    constructor() {
        super();
        this.consumeContext(UMB_AUTH_CONTEXT, ctx => {
            this.#authContext = ctx;
        });

        this.consumeContext(UMB_ACTION_EVENT_CONTEXT, eventContext => {
            if (!eventContext) return;
            eventContext.addEventListener(UmbRequestReloadStructureForEntityEvent.TYPE, event => {
                const reloadEvent = event as unknown as UmbRequestReloadStructureForEntityEvent;
                if (reloadEvent.getEntityType() === UMB_DOCUMENT_ENTITY_TYPE) {
                    this.handleRefresh();
                }
            });
        });
    }

    updated(changed: PropertyValues) {
        if (changed.has('focusedBlockId') && this.focusedBlockId) {
            this._focusBlockInPreview(this.focusedBlockId);
        }
    }

    async firstUpdated() {
        const token = await this.#authContext?.getLatestToken();
        if (token) {
            this.token = token;
            this.ok = true;
        }

        await this.updateComplete;

        this.loadIframe();
        this.setPreviewScale();

        this.#resizeObserver = new ResizeObserver(() => this.setPreviewScale());
        this.#resizeObserver.observe(this);

        this.addEventListener('resize', this._handleWindowResize);
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.#resizeObserver?.disconnect();
        this.removeEventListener('resize', this._handleWindowResize);
    }

    private _handleWindowResize = () => {
        this.setPreviewScale();
    };

    /**
     * Loads or reloads the iframe with the current preview URL.
     */
    private loadIframe() {
        if (!this.iframe) return;

        this.iframe.onload = () => {
            this._lastFocusedBlock = undefined;
            if (this.focusedBlockId) {
                setTimeout(() => {
                    this._focusBlockInPreview(this.focusedBlockId!);
                }, 150);
            }
        };

        const cacheBuster = `&_t=${Date.now()}`;
        this.iframe.src = this.previewUrl + cacheBuster;

        this.lastUpdate = new Date();
    }

    /**
     * Calculates and applies scale transform to iframe based on container width.
     */
    private setPreviewScale() {
        if (!this.iframeFrame || !this.iframe) return;

        const containerWidth = this.iframeFrame.clientWidth;
        const iframeNaturalWidth = this.iframeWidth;
        const ratio = containerWidth / iframeNaturalWidth;

        this.iframe.style.transform = `scale(${ratio}) translateZ(0)`;
        this.iframe.style.transformOrigin = 'top left';
        this.iframe.style.width = `${iframeNaturalWidth}px`;
        this.iframe.style.height = `${this.iframeHeight}px`;

        this.iframeFrame.style.height = `${this.iframeHeight * ratio}px`;
    }

    /**
     * Posts a message to the iframe to scroll to a specific block.
     */
    private _focusBlockInPreview(blockId: string) {
        if (!this.iframe?.contentWindow) return;
        if (this._lastFocusedBlock === blockId) return;

        this.iframe.contentWindow.postMessage({ blockId }, window.location.origin);
        this._lastFocusedBlock = blockId;
    }

    /**
     * Switches between desktop and mobile preview modes.
     */
    private switchTo(mode: 'desktop' | 'mobile') {
        if (this.previewMode === mode) return;

        this.previewMode = mode;

        requestAnimationFrame(() => {
            this.setPreviewScale();

            if (this.focusedBlockId) {
                this._lastFocusedBlock = undefined;
                this._focusBlockInPreview(this.focusedBlockId);
            }
        });
    }

    private switchToDesktop() {
        this.switchTo('desktop');
    }

    private switchToMobile() {
        this.switchTo('mobile');
    }

    render() {
        if (!this.ok) return nothing;

        return html`
            <div class="preview-wrapper">
                <div class="preview-header">
                    <strong class="preview-title">Smart Preview</strong>
                    <div class="preview-controls">
                        <button
                            class="preview-btn ${this.previewMode === 'desktop' ? 'active' : ''}"
                            @click=${this.switchToDesktop}
                            title="Desktop preview"
                        >
                            <uui-icon name="icon-display"></uui-icon>
                            Desktop
                        </button>
                        <button
                            class="preview-btn ${this.previewMode === 'mobile' ? 'active' : ''}"
                            @click=${this.switchToMobile}
                            title="Mobile preview"
                        >
                            <uui-icon name="icon-iphone"></uui-icon>
                            Mobile
                        </button>
                    </div>
                </div>

                <div class="iframe-frame ${this.previewMode}">
                    <iframe
                        class="preview-frame"
                        frameborder="0"
                        scrolling="yes"
                    ></iframe>
                </div>

                ${this.lastUpdate
                    ? html`
                          <div class="preview-footer">
                              <span class="last-update"> Last updated: ${this.formattedLastUpdate} </span>
                          </div>
                      `
                    : nothing}
            </div>
        `;
    }

    private handleRefresh() {
        this._lastFocusedBlock = undefined;
        this.loadIframe();
    }

    static styles = css`
        :host {
            display: block;
            position: relative;
            width: 100%;
        }

        .preview-wrapper {
            display: flex;
            flex-direction: column;
        }

        .preview-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: calc(var(--s, 4px) * 2) calc(var(--s, 4px) * 4);
            background-color: var(--c-mystic, #fcfcfc);
        }

        .preview-title {
            font-size: var(--fs-base, 16px);
            color: var(--c-black, #212121);
        }

        .preview-controls {
            display: flex;
            gap: calc(var(--s, 4px) * 1);
        }

        .preview-btn {
            display: inline-flex;
            align-items: center;
            gap: calc(var(--s, 4px) * 1);
            padding: calc(var(--s, 4px) * 1.5) calc(var(--s, 4px) * 3);
            border: 1px solid rgba(var(--c-submarine, 190, 190, 190), 0.5);
            border-radius: var(--r-base, 2px);
            background-color: transparent;
            color: var(--c-black, #212121);
            cursor: pointer;
            font-size: var(--fs-sm, 14px);
            transition: all 150ms ease;
        }

        .preview-btn:hover {
            background-color: rgba(var(--c-submarine, 190, 190, 190), 0.2);
        }

        .preview-btn.active {
            background-color: var(--c-black, #212121);
            color: var(--c-mystic, #fcfcfc);
            border-color: var(--c-black, #212121);
        }

        .iframe-frame {
            position: relative;
            overflow: hidden;
            background-color: var(--c-mystic, #fcfcfc);
            margin: 0 auto;
            transition: width 250ms ease;
        }

        .iframe-frame.desktop {
            width: 100%;
        }

        .iframe-frame.mobile {
            width: 100%;
        }

        .preview-frame {
            position: absolute;
            top: 0;
            left: 0;
            border: none;
            background-color: #fff;
        }

        .preview-footer {
            display: flex;
            justify-content: center;
            padding: calc(var(--s, 4px) * 1) calc(var(--s, 4px) * 2);
            background-color: var(--c-mystic, #fcfcfc);
        }

        .last-update {
            font-size: var(--fs-xs, 12px);
            color: var(--c-black, #212121);
        }
    `;
}
