import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    css,
    customElement,
    html,
    property,
    query,
    state,
    PropertyValues,
    nothing,
    repeat,
} from '@umbraco-cms/backoffice/external/lit';
import {
    type UmbPropertyEditorConfigCollection,
    UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyTypeModel } from '@umbraco-cms/backoffice/content-type';
import { UmbDataPathPropertyValueQuery } from '@umbraco-cms/backoffice/validation';
import { UMB_PROPERTY_CONTEXT, UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import { UMB_CONTENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/content';
import { fetchDefinitionsPerCategory, fetchPagePresets } from '../queries/definitions.ts';
import { connect } from 'pwa-helpers';
import { AppState, store } from '../state/store.ts';
import { setDefinitions } from '../state/slices/definitions.ts';
import {
    PCBCategoryWithDefinitions,
    PerplexContentBlocksBlock,
    PerplexContentBlocksValue,
    Section,
    Structure,
} from '../types.ts';
import { setIsTouchDevice } from '../state/slices/ui.ts';
import { ON_ADD_TOAST, ToastEvent } from '../events/toast.ts';
import { addToast } from '../utils/toast.ts';
import {
    BlockCreation,
    ON_BLOCK_SAVED,
    ON_BLOCK_TOGGLE,
    ON_BLOCK_UPDATED,
    ON_SET_BLOCKS,
    SetBlocksEvent,
} from '../events/block.ts';

import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { register } from 'swiper/element/bundle';
import { ON_VALUE_COPIED, ON_VALUE_PASTE, ValuePastedEvent } from '../events/copyPaste.ts';
import { CopyPasteState, setCopiedValue } from '../state/slices/copyPaste.ts';
import { setPresets } from '../state/slices/presets.ts';
import { getBlocksFromPreset } from '../utils/preset.ts';
import { provide } from '@lit/context';
import { editorContext } from '../context/index.ts';
import { animate } from '@lit-labs/motion';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { PCB_ADD_BLOCK_MODAL_TOKEN } from '../components/modals/addBlock/modal-token.ts';
import { firstValueFrom } from '@umbraco-cms/backoffice/external/rxjs';

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
    copiedValue?: CopyPasteState;

    editorId: string = crypto.randomUUID();

    @provide({ context: editorContext })
    providedEditorId = this.editorId;

    headerCategories: string[] = [];

    #documentTypeAlias: string = '';

    get structure(): Structure {
        const value = this.config?.getValueByAlias('structure');
        switch (value) {
            case Structure.All:
            case Structure.Blocks:
            case Structure.Header:
                return value as Structure;
            default:
                return Structure.All;
        }
    }

    async fetchDefinitionsPerCategory() {
        const result = await fetchDefinitionsPerCategory(this.#documentTypeAlias, this.culture || undefined);

        if (result) {
            this.headerCategories = result.reduce((acc: string[], currentValue) => {
                if (currentValue.category.isEnabledForHeaders) {
                    acc.push(currentValue.category.id);
                }

                return acc;
            }, []);
            store.dispatch(setDefinitions(result));
            this.requestUpdate();
        }
    }

    async fetchPresets() {
        const result = await fetchPagePresets(this.#documentTypeAlias, this.culture || undefined);

        if (result) {
            store.dispatch(setPresets(result));

            if (result && this.definitions && this.definitions.length > 0) {
                const presetBlocks = getBlocksFromPreset(result, this.definitions, this._value);
                if (presetBlocks.header) {
                    this.addBlocks([presetBlocks.header], Section.HEADER, 0);
                }

                if (presetBlocks.blocks.length > 0) {
                    this.addBlocks(presetBlocks.blocks, Section.CONTENT, 0);
                }
            }

            this.requestUpdate();
        }
    }

    stateChanged(state: AppState) {
        this.definitions = state.definitions.value;
        this.copiedValue = state.copyPaste;
    }

    async connectedCallback() {
        super.connectedCallback();

        const workspaceContext = await this.getContext(UMB_CONTENT_WORKSPACE_CONTEXT);
        this.#documentTypeAlias = (await firstValueFrom(workspaceContext!.structure.ownerContentTypeAlias)) ?? '';

        await this.fetchDefinitionsPerCategory();
        await this.fetchPresets();

        this.addEventListener(ON_ADD_TOAST, (e: Event) => {
            addToast(e as CustomEvent, this);

            this._notificationsElement?.hidePopover?.();
            this._notificationsElement?.showPopover?.();
        });

        const isTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
        store.dispatch(setIsTouchDevice(isTouch));

        this.addEventListener(ON_BLOCK_SAVED, (e: Event) => this.onBlockAdded(e as CustomEvent));
        this.addEventListener(ON_BLOCK_TOGGLE, (e: Event) => this.onBlockToggled(e as CustomEvent));
        this.addEventListener(ON_BLOCK_UPDATED, (e: Event) => this.updateBlock(e as CustomEvent));
        this.addEventListener(ON_VALUE_COPIED, (e: Event) => this.onValueCopied(e as CustomEvent));
        this.addEventListener(ON_VALUE_PASTE, (e: Event) => this.onValuePasted(e as CustomEvent));
        this.addEventListener(ON_SET_BLOCKS, (e: Event) => this.onSetBlocks(e as CustomEvent));
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
        this.removeEventListener(ON_VALUE_COPIED, (e: Event) => this.onValueCopied(e as CustomEvent));
        this.removeEventListener(ON_VALUE_PASTE, (e: Event) => this.onValuePasted(e as CustomEvent));
        this.removeEventListener(ON_SET_BLOCKS, (e: Event) => this.onSetBlocks(e as CustomEvent));
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
    }

    valueChanged() {
        this.dispatchEvent(new UmbChangeEvent());
    }

    addHeader() {
        this._openModal(Section.HEADER);
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
        this._openModal(Section.CONTENT);
    }

    onBlockToggled(event: CustomEvent) {
        if (!this.openedBlocks.includes(event.detail.id)) {
            this.openedBlocks = [...this.openedBlocks, event.detail.id];
        } else {
            this.openedBlocks = this.openedBlocks.filter((id) => id !== event.detail.id);
        }
    }

    addBlocks(blocks: PerplexContentBlocksBlock[], section: Section, desiredIndex: number | null) {
        this.openedBlocks = [...this.openedBlocks, ...blocks.map((b: PerplexContentBlocksBlock) => b.id)];

        if (section === Section.HEADER && blocks.length > 0) {
            const definition = this.findDefinitionById(blocks[0].definitionId);
            if (definition?.categoryIds.some((id) => this.headerCategories.includes(id))) {
                this._value = { ...this._value, header: blocks[0] };
            } else {
                this.dispatchEvent(
                    ToastEvent('warning', {
                        headline: 'This block cannot be added as a header',
                    }),
                );
            }
        } else {
            const contentBlocks = blocks.filter((b: PerplexContentBlocksBlock) => {
                const def = this.findDefinitionById(b.definitionId);
                if (!def) return false;
                // Do not allow header-only blocks as content.
                return !def.categoryIds.every((id) => this.headerCategories.includes(id));
            });

            if (contentBlocks.length < blocks.length) {
                this.dispatchEvent(
                    ToastEvent('warning', {
                        headline: 'Header blocks cannot be added as content and will be ignored',
                    }),
                );
            }

            let updatedBlocks;
            if (typeof desiredIndex === 'number' && desiredIndex >= 0) {
                updatedBlocks = [...this._value.blocks];
                updatedBlocks.splice(desiredIndex, 0, ...contentBlocks);
            } else {
                updatedBlocks = [...this._value.blocks, ...contentBlocks];
            }

            this._value = { ...this._value, blocks: updatedBlocks };
        }

        this.valueChanged();
    }

    onBlockAdded(e: Event) {
        const event = e as CustomEvent<BlockCreation>;
        this.addBlocks(event.detail.blocks, event.detail.section, event.detail.desiredIndex);
    }

    onSetBlocks(event: ReturnType<typeof SetBlocksEvent>) {
        this._value = { ...this._value, blocks: event.detail.blocks };
        this.valueChanged();
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

    onValueCopied(event: CustomEvent) {
        store.dispatch(setCopiedValue(event.detail));
    }

    onValuePasted(event: CustomEvent) {
        if (event.detail.pastedValue) {
            this.addBlocks(event.detail.pastedValue, event.detail.section, event.detail.desiredIndex);
        }
    }

    findDefinitionById(id: string) {
        if (!Array.isArray(this.definitions)) {
            return null;
        }
        const found = this.definitions.map((cat) => cat.definitions[id]).find((def) => def !== undefined);
        return found || null;
    }

    pasteBlock(section: Section) {
        if (this.copiedValue?.copied) {
            this.dispatchEvent(ValuePastedEvent(this.copiedValue.copied, section));
        }
    }

    private _openModal = async (section: Section, insertAtIndex?: number) => {
        const returnedValue = await umbOpenModal(this, PCB_ADD_BLOCK_MODAL_TOKEN, {
            data: {
                editorId: this.editorId,
                groupedDefinitions: this.definitions,
                section,
                insertAtIndex,
            },
        }).catch(() => undefined);
        if (!returnedValue) return;

        this.addBlocks(returnedValue.blocks, returnedValue.section, returnedValue.desiredIndex);
    };

    render() {
        return html`
            <div class="main">
                <div class="pcb__wrapper">
                    <div class="pcb__content">
                        <div class="pcb__blocks">
                            ${this._value.header && this.structure !== Structure.Blocks
                                ? html`
                                      <pcb-block
                                          .draggable=${false}
                                          .block=${this._value.header}
                                          .collapsed=${!this.openedBlocks.includes(this._value.header.id)}
                                          .removeBlock=${this.removeHeader.bind(this)}
                                          .dataPath=${this.dataPath}
                                          .definition=${this.findDefinitionById(this._value.header.definitionId)}
                                          .section=${Section.HEADER}
                                          .openModal=${this._openModal}
                                      ></pcb-block>
                                  `
                                : nothing}
                            ${!this._value.header && this.structure !== Structure.Blocks
                                ? html`
                                      <div class="pcb__block-add pcb__block-add--header">
                                          <uui-button
                                              label="add header"
                                              look="primary"
                                              @click=${this.addHeader}
                                          >
                                              <slot name="label">Add header</slot>
                                              <slot name="extra">
                                                  <uui-icon name="icon-add"></uui-icon>
                                              </slot>
                                          </uui-button>

                                          ${this.copiedValue?.copied
                                              ? html`
                                                    <uui-button
                                                        label="paste header"
                                                        look="primary"
                                                        @click=${() => this.pasteBlock(Section.HEADER)}
                                                    >
                                                        <slot name="label">Paste header</slot>
                                                        <slot name="extra">
                                                            <uui-icon name="icon-clipboard-paste"></uui-icon>
                                                        </slot>
                                                    </uui-button>
                                                `
                                              : nothing}
                                      </div>
                                  `
                                : nothing}
                            ${this.structure !== Structure.Header
                                ? html`
                                      <pcb-drag-and-drop .blocks="${this._value.blocks}">
                                          ${repeat(
                                              this._value.blocks,
                                              (block) => block.id,
                                              (block, index) => html`
                                                  <pcb-drag-item
                                                      .canDrag=${!this.openedBlocks.includes(block.id)}
                                                      .blockId=${block.id}
                                                  >
                                                      <pcb-block
                                                          .draggable=${!this.openedBlocks.includes(block.id)}
                                                          .block=${block}
                                                          .collapsed=${!this.openedBlocks.includes(block.id)}
                                                          .removeBlock=${this.removeBlock.bind(this)}
                                                          .dataPath=${this.dataPath}
                                                          .definition=${this.findDefinitionById(block.definitionId)}
                                                          .section=${Section.CONTENT}
                                                          .index=${index}
                                                          ${animate({ id: block.id })}
                                                          .openModal=${this._openModal}
                                                      ></pcb-block>
                                                  </pcb-drag-item>
                                              `,
                                          )}
                                      </pcb-drag-and-drop>
                                  `
                                : nothing}
                        </div>

                        ${this.structure !== Structure.Header
                            ? html`
                                  <div class="pcb__block-add">
                                      <uui-button
                                          label="add content"
                                          look="primary"
                                          @click=${this.addBlock}
                                      >
                                          <slot name="label">Add content</slot>
                                          <slot name="extra">
                                              <uui-icon name="icon-add"></uui-icon>
                                          </slot>
                                      </uui-button>

                                      ${this.copiedValue?.copied
                                          ? html`
                                                <uui-button
                                                    look="primary"
                                                    label="paste content"
                                                    @click=${() => this.pasteBlock(Section.CONTENT)}
                                                >
                                                    <slot name="label">Paste content</slot>
                                                    <slot name="extra">
                                                        <uui-icon name="icon-clipboard-paste"></uui-icon>
                                                    </slot>
                                                </uui-button>
                                            `
                                          : nothing}
                                  </div>
                              `
                            : nothing}
                    </div>
                </div>

                ${this.config?.getValueByAlias('debug') === true
                    ? html`
                          <div class="debug">
                              <uui-button
                                  look="outline"
                                  label="raw value"
                                  @click=${() => (this.showDebug = !this.showDebug)}
                              ></uui-button>
                              ${this.showDebug ? html` <pre>${JSON.stringify(this.value, null, 4)}</pre>` : nothing}
                          </div>
                      `
                    : nothing}
            </div>

            <uui-toast-notification-container
                auto-close="7000"
                bottom-up
                id="notifications"
                popover="manual"
                style="z-index: 2000; padding: var(--uui-size-layout-1);"
            ></uui-toast-notification-container>
        `;
    }

    static styles = [
        css`
            :host {
                display: grid;
                gap: 1rem;

                @media only screen and (min-width: 1800px) {
                    grid-template-columns: 3fr 1fr;
                }
            }

            pcb-drag-and-drop {
                display: flex;
                flex-direction: column;
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

            .main {
                box-sizing: border-box;
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
                    gap: calc(var(--s) * 3);

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
