import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    html,
    customElement,
    property,
    repeat,
    css,
    state,
    nothing,
    query,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import { type UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyTypeModel } from '@umbraco-cms/backoffice/content-type';
import { UmbDataPathPropertyValueQuery } from '@umbraco-cms/backoffice/validation';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import { UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import { fetchDefinitionsPerCategory } from './queries/definitions.ts';
import { connect } from 'pwa-helpers';
import { store } from './state/store.ts';
import { setDefinitions } from './state/slices/definitions.ts';
import { PerplexContentBlocksBlock, PerplexContentBlocksValue } from './types.ts';
import { setAddBlockModal, resetAddBlockModal } from './state/slices/ui.ts';
import { ON_ADD_TOAST } from './events/toast.ts';
import { addToast } from './utils/toast.ts';
import { ON_BLOCK_SAVED, ON_BLOCK_TOGGLE } from './events/block.ts';
import { animate } from '@lit-labs/motion';
import { UMB_AUTH_CONTEXT, UmbAuthContext } from '@umbraco-cms/backoffice/auth';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

@customElement('perplex-content-blocks')
export default class PerplexContentBlocksElement
    extends connect(store)(UmbLitElement)
    implements UmbPropertyEditorUiElement
{
    @query('#notifications')
    private _notificationsElement?: HTMLElement;

    @state()
    properties: UmbPropertyTypeModel[] | undefined = undefined;

    @state()
    openedBlocks: string[] = [];

    @state()
    showDebug: boolean = false;

    @state()
    private _value: PerplexContentBlocksValue = {
        version: 4,
        header: null,
        blocks: [],
    };

    @property({ attribute: false })
    public set value(value: PerplexContentBlocksValue | undefined) {
        if (value == null) {
            return;
        }

        this._value = value;
    }

    public get value(): PerplexContentBlocksValue {
        return this._value;
    }

    @property({ attribute: false })
    config: UmbPropertyEditorConfigCollection | undefined;

    @state()
    dataPath!: string;

    @state()
    pageId!: string;

    @state()
    culture!: string | null;

    @state()
    definitions = [];

    @state()
    categories = [];

    #authContext?: UmbAuthContext;

    async fetchDefinitonsPerCategory() {
        const token = await this.#authContext?.getLatestToken();
        if (token == null) {
            throw new Error('No auth token available');
        }

        const result = await fetchDefinitionsPerCategory(token);

        if (result) {
            store.dispatch(setDefinitions(result));
            this.requestUpdate();
        }
    }

    connectedCallback() {
        super.connectedCallback();
        this.fetchDefinitonsPerCategory();
        this.addEventListener(ON_ADD_TOAST, (e: Event) => {
            addToast(e as CustomEvent, this);

            this._notificationsElement?.hidePopover?.();
            this._notificationsElement?.showPopover?.();
        });

        this.addEventListener(ON_BLOCK_SAVED, (e: Event) => this.onBlockAdded(e as CustomEvent));
        this.addEventListener(ON_BLOCK_TOGGLE, (e: Event) => this.onBlockToggled(e as CustomEvent));
    }

    constructor() {
        super();
        this.consumeContext(UMB_PROPERTY_CONTEXT, (ctx) => {
            const alias = ctx?.getAlias() || '';
            const culture = ctx?.getVariantId()?.culture;
            this.dataPath = `$.values[${UmbDataPathPropertyValueQuery({ alias, culture })}].value`;
        });

        this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, (ctx) => {
            this.pageId = ctx?.getUnique()!;
            this.culture = ctx?.getVariantId().culture || '';
        });

        this.consumeContext(UMB_AUTH_CONTEXT, (ctx) => {
            this.#authContext = ctx;
        });
    }

    valueChanged() {
        this.dispatchEvent(new UmbChangeEvent());
    }

    addHeader() {
        store.dispatch(
            setAddBlockModal({
                display: true,
                section: 'header',
            }),
        );
    }

    updateHeader(header: PerplexContentBlocksBlock) {
        this._value = { ...this._value, header };
        this.valueChanged();
    }

    removeHeader() {
        this._value = { ...this._value, header: null };
        this.valueChanged();
    }

    addBlock() {
        store.dispatch(
            setAddBlockModal({
                display: true,
                section: 'content',
            }),
        );
    }

    onBlockToggled(event: CustomEvent) {
        if (!this.openedBlocks.includes(event.detail.id)) {
            this.openedBlocks = [...this.openedBlocks, event.detail.id];
        } else {
            this.openedBlocks = this.openedBlocks.filter((id) => id !== event.detail.id);
        }
    }

    onBlockAdded(event: CustomEvent) {
        if (event.detail.section === 'header') {
            this.updateHeader(event.detail.block);
            this.openedBlocks = [...this.openedBlocks, event.detail.block.id];
            store.dispatch(resetAddBlockModal());
            return;
        }

        const blocks = [...this._value.blocks, event.detail.block];
        this.openedBlocks = [...this.openedBlocks, event.detail.block.id];
        this._value = { ...this._value, blocks };

        this.valueChanged();

        store.dispatch(setAddBlockModal(false));
    }

    removeBlock(key: string) {
        const blocks = this._value.blocks.filter((block) => block.content.key !== key);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    updateBlock(block: PerplexContentBlocksBlock) {
        const idx = this._value.blocks.findIndex((b) => b.id === block.id);
        if (idx === -1) {
            return;
        }

        const blocks = [...this._value.blocks];
        blocks.splice(idx, 1, block);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    render() {
        return html`
            <div class="main">
                <div class="pcb__wrapper">
                    <div class="pcb__headers">
                        <h2>Header</h2>

                            ${
                                (this._value.header &&
                                    html` <div
                                        class="pcb__blocks"
                                        ${animate()}
                                    >
                                        <pcb-block
                                            .block=${this._value.header}
                                            .collapsed="${!this.openedBlocks.includes(this._value.header.id)}"
                                            .removeBlock=${this.removeHeader.bind(this)}
                                            .onChange=${this.updateHeader.bind(this)}
                                            .dataPath=${this.dataPath}
                                        ></pcb-block>
                                    </div>`) ||
                                nothing
                            }
                        ${
                            (!this._value.header &&
                                html` <div class="pcb__headers-add">
                                    <uui-button
                                        look="primary"
                                        @click=${this.addHeader}
                                    >
                                        <slot name="label"> Add header</slot>

                                        <slot name="extra">
                                            <uui-icon name="icon-add"></uui-icon>
                                        </slot>
                                    </uui-button>
                                </div>`) ||
                            nothing
                        }

                    </div>
                    <div class="pcb__content">
                        <h2>Content</h2>
                        <div class="pcb__blocks" ${animate()}>
                            ${repeat(
                                this._value.blocks,
                                (block) => block.id,
                                (block) =>
                                    html` <pcb-block
                                        .block=${block}
                                        .collapsed="${!this.openedBlocks.includes(block.id)}"
                                        .removeBlock=${this.removeBlock.bind(this)}
                                        .onChange=${this.updateBlock.bind(this)}
                                        .dataPath=${this.dataPath}
                                        ${animate({ id: block.id })}
                                    ></pcb-block>`,
                            )}
                        </div>
                        <div class="pcb__block-add">
                            <uui-button look="primary" @click=${this.addBlock}>

                                <slot name="label">
                                    Add content
                                </slot>

                                <slot name="extra">
                                    <uui-icon name="icon-add"></uui-icon>
                                </slot>
                            </uui-button>
                        </div>
                    </div>
                </div>
                <div class="debug">
                    <uui-button
                            look="outline"
                            label="raw value"
                            @click=${() => (this.showDebug = !this.showDebug)}
                    ></uui-button>
                    ${(this.showDebug && html` <pre>${JSON.stringify(this.value, null, 4)}</pre>`) || null}
                </div>
            </div>

            <uui-box
                    headline="Smart Preview"
                    headline-variant="h5"
            >
                <div class="sidebar">
                    <pcb-preview
                            .culture=${this.culture}
                            .pageId=${this.pageId}
                    ></pcb-preview>
                </div>
            </uui-box>
            <uui-toast-notification-container
                    auto-close="7000"
                    bottom-up=""
                    id="notifications"
                    popover="manual"
                    style="z-index: 2000;"
                    padding: var(--uui-size-layout-1);
            >
            </uui-toast-notification-container>

            <pcb-add-block-modal>
            </pcb-add-block-modal>


        `;
    }

    static styles = [
        css`
            :host {
                display: grid;
                grid-template-columns: 3fr 1fr;
                gap: 1rem;
                overflow: hidden;
            }

            #notifications {
                top: 0;
                left: 0;
                right: 0;
                bottom: 45px;
                height: auto;
                padding: var(--uui-size-layout-1);

                position: fixed;
                width: 100vw;
                background: 0;
                outline: 0;
                border: 0;
                margin: 0;
            }

            .main,
            .sidebar {
                padding: 1rem 1.5rem;
            }

            .pcb__wrapper {
                display: flex;
                flex-direction: column;
                gap: 2rem;
            }

            .pcb__region {
                background-color: var(--c-wild-sand);
            }

            .pcb__blocks {
                display: grid;
                grid-template-columns: 1fr;
                gap: 1rem;
                padding-block-end: 1rem;
            }

            .debug {
                margin-top: 1rem;
            }

            .pcb__content {
                .pcb__block-add {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }
            }

            .pcb__headers {
                .pcb__headers-add {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }
            }
        `,
    ];
}
