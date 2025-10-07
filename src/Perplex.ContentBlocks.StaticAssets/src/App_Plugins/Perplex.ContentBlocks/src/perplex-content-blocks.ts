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
import { PCBCategoryWithDefinitions, PerplexContentBlocksBlock, PerplexContentBlocksValue, Section } from './types.ts';
import { setAddBlockModal, resetAddBlockModal } from './state/slices/ui.ts';
import { ON_ADD_TOAST } from './events/toast.ts';
import { addToast } from './utils/toast.ts';
import { ON_BLOCK_SAVED, ON_BLOCK_TOGGLE, ON_BLOCK_UPDATED } from './events/block.ts';
import { animate } from '@lit-labs/motion';
import { UMB_AUTH_CONTEXT, UmbAuthContext } from '@umbraco-cms/backoffice/auth';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { PropertyValues } from 'lit';
import { register } from 'swiper/element/bundle';

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
    definitions: PCBCategoryWithDefinitions[] = [];

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

    stateChanged(state: any) {
        this.definitions = state.definitions.value;
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
        this.addEventListener(ON_BLOCK_UPDATED, (e: Event) => this.updateBlock(e as CustomEvent));
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.removeEventListener(ON_ADD_TOAST, (e: Event) => {
            addToast(e as CustomEvent, this);

            this._notificationsElement?.hidePopover?.();
            this._notificationsElement?.showPopover?.();
        });
        this.removeEventListener(ON_BLOCK_SAVED, (e: Event) => this.onBlockAdded(e as CustomEvent));
        this.removeEventListener(ON_BLOCK_TOGGLE, (e: Event) => this.onBlockToggled(e as CustomEvent));
        this.removeEventListener(ON_BLOCK_UPDATED, (e: Event) => this.updateBlock(e as CustomEvent));
    }

    protected firstUpdated(_changedProperties: PropertyValues) {
        register();
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
                section: Section.HEADER,
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
                section: Section.CONTENT,
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
        if (event.detail.section === Section.HEADER) {
            this.updateHeader(event.detail.block);
            this.openedBlocks = [...this.openedBlocks, event.detail.block.id];
            store.dispatch(resetAddBlockModal());
            return;
        }

        // if event contains detail desired index, insert block at that index
        let blocks;
        if (event.detail.desiredIndex !== undefined) {
            blocks = [...this._value.blocks];
            blocks.splice(event.detail.desiredIndex, 0, event.detail.block);
        } else {
            blocks = [...this._value.blocks, event.detail.block];
        }

        this.openedBlocks = [...this.openedBlocks, event.detail.block.id];
        this._value = { ...this._value, blocks };

        this.valueChanged();

        store.dispatch(setAddBlockModal(false));
    }

    removeBlock(id: string) {
        const blocks = this._value.blocks.filter((block) => block.id !== id);

        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    updateBlock(event: CustomEvent) {
        if (event.detail.section === Section.HEADER) {
            this.updateHeader(event.detail.block);
            return;
        }

        const idx = this._value.blocks.findIndex((b) => b.id === event.detail.block.id);
        if (idx === -1) {
            return;
        }

        const blocks = [...this._value.blocks];
        blocks.splice(idx, 1, event.detail.block);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    findDefinitionById(id: string) {
        if (!Array.isArray(this.definitions)) {
            return null;
        }
        const found = this.definitions.map((cat) => cat.definitions[id]).find((def) => def !== undefined);
        return found || null;
    }

    render() {
        return html`
            <div class="main">
                <div class="pcb__wrapper">
                    <div class="pcb__content">
                        <div class="pcb__blocks">
                            ${
                                (this._value.header &&
                                    html` <div ${animate()}>
                                        <pcb-block
                                            .block=${this._value.header}
                                            .collapsed="${!this.openedBlocks.includes(this._value.header.id)}"
                                            .removeBlock=${this.removeHeader.bind(this)}
                                            .dataPath=${this.dataPath}
                                            .definition=${this.findDefinitionById(this._value.header.definitionId)}
                                            .section=${Section.HEADER}
                                        ></pcb-block>
                                    </div>`) ||
                                nothing
                            }
                            ${
                                (!this._value.header &&
                                    html` <div class="pcb__block-add pcb__block-add--header">
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
                            ${repeat(
                                this._value.blocks,
                                (block) => block.id,
                                (block, index) => html`
                                    ${!this._value.header && index === 0
                                        ? nothing
                                        : html` <pcb-block-spacer .index="${index}"></pcb-block-spacer>`}
                                    <pcb-block
                                        .block=${block}
                                        .collapsed="${!this.openedBlocks.includes(block.id)}"
                                        .removeBlock=${this.removeBlock.bind(this)}
                                        .dataPath=${this.dataPath}
                                        .definition=${this.findDefinitionById(block.definitionId)}
                                        .section=${Section.CONTENT}
                                        ${animate({ id: block.id })}
                                    ></pcb-block>
                                `,
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
                gap: 1rem;
                min-height: 40rem;

                @media only screen and (min-width: 1800px) {
                    grid-template-columns: 3fr 1fr;
                }
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
                display: block;
            }

            .pcb__region {
                background-color: var(--c-wild-sand);
            }

            .pcb__blocks {
                display: grid;
                grid-template-columns: 1fr;
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
                    padding: calc(var(--s) * 3);
                    background-color: var(--c-mystic);

                    &.pcb__block-add--header {
                        margin-block-end: 1rem;
                    }
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
