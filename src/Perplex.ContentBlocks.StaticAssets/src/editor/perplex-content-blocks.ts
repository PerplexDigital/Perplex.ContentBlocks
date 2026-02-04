import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    css,
    customElement,
    html,
    property,
    query,
    state,
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
    PerplexBlockDefinition,
    PerplexContentBlocksBlock,
    PerplexContentBlocksValue,
    Section,
    Structure,
} from '../types.ts';
import { setIsTouchDevice } from '../state/slices/ui.ts';
import { PcbToastEvent } from '../events/toast.ts';
import { addToast } from '../utils/toast.ts';
import { PcbBlockSavedEvent, PcbBlockToggleEvent, PcbBlockUpdatedEvent, PcbSetBlocksEvent } from '../events/block.ts';

import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { PcbValueCopiedEvent, PcbValuePastedEvent } from '../events/copyPaste.ts';
import { CopyPasteState, setCopiedValue } from '../state/slices/copyPaste.ts';
import { setPresets } from '../state/slices/presets.ts';
import { getBlocksFromPreset } from '../utils/preset.ts';
import { differentiateBlocks } from '../utils/copyPaste.ts';
import { provide } from '@lit/context';
import { editorContext } from '../context/index.ts';
import { animate } from '@lit-labs/motion';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { PCB_ADD_BLOCK_MODAL_TOKEN } from '../components/modals/addBlock/modal-token.ts';
import { firstValueFrom } from '@umbraco-cms/backoffice/external/rxjs';
import { PcbFocusBlockInPreviewEvent } from '../events/preview.ts';

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

    private readonly eventHandlers = new Map<string, EventListener>([
        [PcbBlockSavedEvent.TYPE, e => this.onBlockAdded(e as PcbBlockSavedEvent)],
        [PcbBlockToggleEvent.TYPE, e => this.onBlockToggled(e as PcbBlockToggleEvent)],
        [PcbBlockUpdatedEvent.TYPE, e => this.updateBlock(e as PcbBlockUpdatedEvent)],
        [PcbValueCopiedEvent.TYPE, e => this.onValueCopied(e as PcbValueCopiedEvent)],
        [PcbValuePastedEvent.TYPE, e => this.onValuePasted(e as PcbValuePastedEvent)],
        [PcbSetBlocksEvent.TYPE, e => this.onSetBlocks(e as PcbSetBlocksEvent)],
        [PcbFocusBlockInPreviewEvent.TYPE, e => this.onFocusBlock(e as PcbFocusBlockInPreviewEvent)],
    ]);

    @property({ attribute: false })
    config: UmbPropertyEditorConfigCollection | undefined;

    @state()
    dataPath!: string;

    @state()
    culture!: string;

    @state()
    pageId!: string;

    @state()
    definitions: PCBCategoryWithDefinitions[] = [];

    @state()
    copiedValue?: CopyPasteState;

    @state()
    private focusedBlockId?: string;

    editorId: string = crypto.randomUUID();

    @provide({ context: editorContext })
    providedEditorId = this.editorId;

    headerCategories: string[] = [];

    #documentTypeAlias: string = '';
    #definitionsMap: Map<string, PerplexBlockDefinition> = new Map();
    #boundRemoveBlock = this.removeBlock.bind(this);

    @property({ attribute: false })
    public set value(value: PerplexContentBlocksValue | undefined) {
        if (value == null) return;
        this._value = value;
    }

    public get value(): PerplexContentBlocksValue {
        return this._value;
    }

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

    async connectedCallback() {
        super.connectedCallback();

        const [propertyCtx, workspaceCtx, datasetCtx] = await Promise.all([
            this.getContext(UMB_PROPERTY_CONTEXT),
            this.getContext(UMB_CONTENT_WORKSPACE_CONTEXT),
            this.getContext(UMB_PROPERTY_DATASET_CONTEXT),
        ]);

        if (!propertyCtx) throw new Error('Property context is required');
        if (!workspaceCtx) throw new Error('Workspace context is required');
        if (!datasetCtx) throw new Error('Dataset context is required');

        const alias = propertyCtx.getAlias() || '';
        this.culture = propertyCtx.getVariantId()?.culture || '';
        this.pageId = datasetCtx.getUnique()! || '';
        this.dataPath = `$.values[${UmbDataPathPropertyValueQuery({ alias, culture: this.culture })}].value`;

        this.#documentTypeAlias = (await firstValueFrom(workspaceCtx.structure.ownerContentTypeAlias)) ?? '';

        await Promise.all([this.fetchDefinitionsPerCategory(), this.fetchPresets()]);

        this.addEventListener(PcbToastEvent.TYPE, (e: Event) => {
            addToast(e as PcbToastEvent, this);
            this._notificationsElement?.hidePopover?.();
            this._notificationsElement?.showPopover?.();
        });

        const isTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
        store.dispatch(setIsTouchDevice(isTouch));

        for (const [type, handler] of this.eventHandlers) {
            this.addEventListener(type, handler);
        }
    }

    disconnectedCallback() {
        super.disconnectedCallback();

        this.removeEventListener(PcbToastEvent.TYPE, (e: Event) => {
            addToast(e as PcbToastEvent, this);
            this._notificationsElement?.hidePopover?.();
            this._notificationsElement?.showPopover?.();
        });

        for (const [type, handler] of this.eventHandlers) {
            this.removeEventListener(type, handler);
        }
    }

    stateChanged(state: AppState) {
        this.definitions = state.definitions.value;
        this.copiedValue = state.copyPaste;

        this.#definitionsMap.clear();
        for (const cat of this.definitions) {
            for (const [id, def] of Object.entries(cat.definitions)) {
                this.#definitionsMap.set(id, def);
            }
        }
    }

    valueChanged() {
        this.dispatchEvent(new UmbChangeEvent());
    }

    private onFocusBlock(e: PcbFocusBlockInPreviewEvent) {
        this.focusedBlockId = e.blockId;
    }

    private get allBlockIds(): string[] {
        const ids: string[] = [];
        if (this._value.header) {
            ids.push(this._value.header.id);
        }
        ids.push(...this._value.blocks.map(b => b.id));
        return ids;
    }

    private get areAllBlocksOpen(): boolean {
        const allIds = this.allBlockIds;
        if (allIds.length === 0) return false;
        return allIds.every(id => this.openedBlocks.includes(id));
    }

    private toggleAllBlocks() {
        if (this.areAllBlocksOpen) {
            this.openedBlocks = [];
        } else {
            this.openedBlocks = [...this.allBlockIds];
        }
    }

    private copyAllBlocks() {
        const header = this._value.header;
        const blocks = this._value.blocks;
        const totalCount = (header ? 1 : 0) + blocks.length;

        if (totalCount === 0) {
            this.dispatchEvent(
                new PcbToastEvent('warning', {
                    headline: 'No blocks to copy',
                }),
            );
            return;
        }

        store.dispatch(setCopiedValue({ header, blocks }));
        this.dispatchEvent(
            new PcbToastEvent('positive', {
                headline: `Copied ${totalCount} block${totalCount > 1 ? 's' : ''} to clipboard`,
            }),
        );
    }

    addHeader() {
        this._openModal(Section.HEADER);
    }

    addBlock() {
        this._openModal(Section.CONTENT);
    }

    updateHeader(header: PerplexContentBlocksBlock) {
        this._value = { ...this._value, header };
        this.valueChanged();
    }

    removeHeader() {
        this._value = { ...this._value, header: null };
        this.valueChanged();
    }

    onBlockToggled(event: PcbBlockToggleEvent) {
        if (!this.openedBlocks.includes(event.id)) {
            this.openedBlocks = [...this.openedBlocks, event.id];
        } else {
            this.openedBlocks = this.openedBlocks.filter(id => id !== event.id);
        }
    }

    onBlockAdded(event: PcbBlockSavedEvent) {
        this.addBlocks(event.blocks, event.section, event.desiredIndex);
    }

    onSetBlocks(event: PcbSetBlocksEvent) {
        this._value = { ...this._value, blocks: event.blocks };
        this.valueChanged();
    }

    updateBlock(event: PcbBlockUpdatedEvent) {
        if (event.section === Section.HEADER) {
            this.updateHeader(event.block);
            return;
        }

        const idx = this._value.blocks.findIndex(b => b.id === event.block.id);
        if (idx === -1) return;

        const blocks = [...this._value.blocks];
        blocks.splice(idx, 1, event.block);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    removeBlock(id: string) {
        const blocks = this._value.blocks.filter(block => block.id !== id);
        this._value = { ...this._value, blocks };
        this.valueChanged();
    }

    onValueCopied(event: PcbValueCopiedEvent) {
        const { blocks, section } = event;
        if (section === Section.HEADER && blocks.length > 0) {
            store.dispatch(setCopiedValue({ header: blocks[0], blocks: [] }));
        } else {
            store.dispatch(setCopiedValue({ header: null, blocks }));
        }
    }

    onValuePasted(event: PcbValuePastedEvent) {
        const { pastedValue, section, desiredIndex } = event;
        if (!pastedValue) return;

        const warnings: string[] = [];
        let headerPasted = false;

        // Create fresh copies with new IDs for each paste operation to avoid shared references
        const freshHeader = pastedValue.header ? differentiateBlocks([pastedValue.header])[0] : null;
        const freshBlocks = differentiateBlocks(pastedValue.blocks);

        if (freshHeader) {
            if (this._value.header) {
                warnings.push('Header was ignored because one already exists');
            } else {
                const headerDef = this.findDefinitionById(freshHeader.definitionId);
                if (headerDef) {
                    this._value = { ...this._value, header: freshHeader };
                    this.openedBlocks = [...this.openedBlocks, freshHeader.id];
                    headerPasted = true;
                } else {
                    warnings.push('Header block is not allowed on this page and was ignored');
                }
            }
        }

        if (freshBlocks.length > 0) {
            this.addBlocks(freshBlocks, section, desiredIndex ?? null);
        }

        if (warnings.length > 0) {
            warnings.forEach(warning => {
                this.dispatchEvent(
                    new PcbToastEvent('warning', {
                        headline: warning,
                    }),
                );
            });
        }

        if (headerPasted && pastedValue.blocks.length === 0) {
            this.valueChanged();
        }
    }

    pasteBlock(section: Section) {
        if (this.copiedValue?.copied) {
            this.dispatchEvent(new PcbValuePastedEvent(this.copiedValue.copied, section));
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

            if (this.definitions?.length > 0) {
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

    findDefinitionById(id: string) {
        return this.#definitionsMap.get(id) || null;
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

    addBlocks(blocks: PerplexContentBlocksBlock[], section: Section, desiredIndex: number | null) {
        const allowedBlocks = blocks.filter(b => {
            const def = this.findDefinitionById(b.definitionId);
            return def !== null;
        });

        const skippedCount = blocks.length - allowedBlocks.length;
        if (skippedCount > 0) {
            this.dispatchEvent(
                new PcbToastEvent('warning', {
                    headline: `${skippedCount} block${skippedCount > 1 ? 's were' : ' was'} not allowed on this page and ignored`,
                }),
            );
        }

        if (allowedBlocks.length === 0) {
            return;
        }

        this.openedBlocks = [...this.openedBlocks, ...allowedBlocks.map(b => b.id)];

        if (section === Section.HEADER && allowedBlocks.length > 0) {
            const definition = this.findDefinitionById(allowedBlocks[0].definitionId);
            if (definition?.categoryIds.some(id => this.headerCategories.includes(id))) {
                this._value = { ...this._value, header: allowedBlocks[0] };
            } else {
                this.dispatchEvent(
                    new PcbToastEvent('warning', {
                        headline: 'This block cannot be added as a header',
                    }),
                );
            }
        } else {
            const contentBlocks = allowedBlocks.filter(b => {
                const def = this.findDefinitionById(b.definitionId);
                if (!def) return false;
                return !def.categoryIds.every(id => this.headerCategories.includes(id));
            });

            if (contentBlocks.length < allowedBlocks.length) {
                this.dispatchEvent(
                    new PcbToastEvent('warning', {
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
                                              block => block.id,
                                              (block, index) => html`
                                                  <pcb-drag-item
                                                      .canDrag=${!this.openedBlocks.includes(block.id)}
                                                      .blockId=${block.id}
                                                  >
                                                      <pcb-block
                                                          .draggable=${!this.openedBlocks.includes(block.id)}
                                                          .block=${block}
                                                          .collapsed=${!this.openedBlocks.includes(block.id)}
                                                          .removeBlock=${this.#boundRemoveBlock}
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
            ${this.pageId
                ? html`
                      <div class="sidebar">
                          <div class="sidebar__section">
                              <pcb-preview
                                  .culture=${this.culture}
                                  .focusedBlockId=${this.focusedBlockId}
                                  .pageId=${this.pageId}
                              ></pcb-preview>
                          </div>
                          <div class="sidebar__section sidebar__controls">
                              <button
                                  class="sidebar__btn"
                                  @click=${this.toggleAllBlocks}
                              >
                                  <uui-icon
                                      name=${this.areAllBlocksOpen ? 'icon-defrag' : 'icon-browser-window'}
                                  ></uui-icon>
                                  ${this.areAllBlocksOpen ? 'Close all blocks' : 'Open all blocks'}
                              </button>
                              <button
                                  class="sidebar__btn"
                                  @click=${this.copyAllBlocks}
                              >
                                  <uui-icon name="icon-documents"></uui-icon>
                                  Copy all blocks
                              </button>
                          </div>
                      </div>
                  `
                : nothing}
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
                align-items: start;

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

            .sidebar {
                display: none;
                flex-direction: column;
                gap: calc(var(--s, 4px) * 3);
                position: sticky;
                top: 0;

                @media only screen and (min-width: 1800px) {
                    display: flex;
                }
            }

            .sidebar__section {
                background-color: var(--c-mystic, #fcfcfc);
                border: 1px solid rgba(var(--c-submarine, 190, 190, 190), 0.5);
                border-radius: var(--r-lg, 4px);
            }

            .sidebar__controls {
                display: flex;
                flex-direction: column;
                gap: calc(var(--s, 4px) * 2);
                padding: calc(var(--s, 4px) * 3);
            }

            .sidebar__btn {
                display: inline-flex;
                align-items: center;
                justify-content: center;
                gap: calc(var(--s, 4px) * 2);
                padding: calc(var(--s, 4px) * 2.5) calc(var(--s, 4px) * 4);
                border: 1px solid rgba(var(--c-submarine, 190, 190, 190), 0.7);
                border-radius: var(--r-base, 2px);
                background-color: var(--c-wild-sand, #f5f5f5);
                color: var(--c-black, #212121);
                cursor: pointer;
                font-size: var(--fs-sm, 14px);
                font-weight: 500;
                transition: all 150ms ease;
            }

            .sidebar__btn:hover {
                background-color: rgba(var(--c-submarine, 190, 190, 190), 0.3);
                border-color: rgba(var(--c-submarine, 190, 190, 190), 1);
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
