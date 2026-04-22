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
import { UmbDataPathPropertyValueQuery } from '@umbraco-cms/backoffice/validation';
import { UMB_PROPERTY_CONTEXT, UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import { UMB_CONTENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/content';
import { fetchDefinitionsPerCategory, fetchPagePresets } from '../queries/definitions.ts';
import { PerplexContentBlocksBlock, PerplexContentBlocksValue, Section, Structure } from '../types.ts';
import { PcbToastEvent } from '../events/toast.ts';
import { addToast } from '../utils/toast.ts';
import { PcbBlockSavedEvent, PcbBlockToggleEvent, PcbBlockUpdatedEvent, PcbSetBlocksEvent } from '../events/block.ts';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { PcbValueCopiedEvent, PcbValuePastedEvent } from '../events/copyPaste.ts';
import { getBlocksFromPreset } from '../utils/preset.ts';
import { differentiateBlocks } from '../utils/copyPaste.ts';
import { provide } from '@lit/context';
import { pcbEditorContext } from '../context/index.ts';
import { PcbEditorContext, CopiedData } from '../context/pcb-editor-context.ts';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { PCB_ADD_BLOCK_MODAL_TOKEN } from '../components/modals/addBlock/modal-token.ts';
import { firstValueFrom } from '@umbraco-cms/backoffice/external/rxjs';
import { PcbFocusBlockInPreviewEvent } from '../events/preview.ts';

@customElement('perplex-content-blocks')
export default class PerplexContentBlocksElement extends UmbLitElement implements UmbPropertyEditorUiElement {
    @query('#notifications')
    private _notificationsElement?: HTMLElement;

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

    @state()
    private _copiedValue: CopiedData | null = null;

    @state()
    private _isDraggingBlock = false;

    @property({ attribute: false })
    config: UmbPropertyEditorConfigCollection | undefined;

    @state()
    dataPath!: string;

    @state()
    culture!: string;

    @state()
    pageId!: string;

    @state()
    private _definitions: import('../types.ts').PCBCategoryWithDefinitions[] = [];

    @state()
    private focusedBlockId?: string;

    // ── Context instances ─────────────────────────────────────────────

    /** Rich context shared to all descendants via @lit/context. */
    @provide({ context: pcbEditorContext })
    ctx!: PcbEditorContext;

    // ── Private fields ────────────────────────────────────────────────

    #documentTypeAlias: string = '';
    readonly #boundRemoveBlock = this.removeBlock.bind(this);

    // ── Event handler map (stable references for add/remove) ─────────

    private readonly _eventHandlers = new Map<string, EventListener>([
        [PcbBlockSavedEvent.TYPE, e => this.onBlockAdded(e as PcbBlockSavedEvent)],
        [PcbBlockToggleEvent.TYPE, e => this.onBlockToggled(e as PcbBlockToggleEvent)],
        [PcbBlockUpdatedEvent.TYPE, e => this.updateBlock(e as PcbBlockUpdatedEvent)],
        [PcbValueCopiedEvent.TYPE, e => this.onValueCopied(e as PcbValueCopiedEvent)],
        [PcbValuePastedEvent.TYPE, e => this.onValuePasted(e as PcbValuePastedEvent)],
        [PcbSetBlocksEvent.TYPE, e => this.onSetBlocks(e as PcbSetBlocksEvent)],
        [PcbFocusBlockInPreviewEvent.TYPE, e => this.onFocusBlock(e as PcbFocusBlockInPreviewEvent)],
        [PcbToastEvent.TYPE, e => this._onToast(e as PcbToastEvent)],
    ]);

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

    // ── Lifecycle ─────────────────────────────────────────────────────

    async connectedCallback() {
        super.connectedCallback();

        // Create the shared context (caches, definitions, etc.)
        this.ctx = new PcbEditorContext(this);

        // Load copied value from session storage
        this._copiedValue = this.ctx.getCopied();

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

        // Fetch definitions and presets in parallel
        await this._fetchData();

        // Register event listeners (stable references)
        for (const [type, handler] of this._eventHandlers) {
            this.addEventListener(type, handler);
        }
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        for (const [type, handler] of this._eventHandlers) {
            this.removeEventListener(type, handler);
        }
    }

    // ── Data fetching ─────────────────────────────────────────────────

    private async _fetchData() {
        const [definitions, presets] = await Promise.all([
            fetchDefinitionsPerCategory(this.#documentTypeAlias, this.culture || undefined),
            fetchPagePresets(this.#documentTypeAlias, this.culture || undefined),
        ]);

        if (definitions) {
            this.ctx.setDefinitions(definitions);
            this._definitions = definitions;
        }

        if (presets) {
            this.ctx.presets = presets;

            if (this._definitions.length > 0) {
                const presetBlocks = getBlocksFromPreset(presets, this._definitions, this._value);

                if (presetBlocks.header) {
                    this.addBlocks([presetBlocks.header], Section.HEADER, 0);
                }
                if (presetBlocks.blocks.length > 0) {
                    this.addBlocks(presetBlocks.blocks, Section.CONTENT, 0);
                }
            }
        }
    }

    // ── Toast handling ────────────────────────────────────────────────

    private _onToast(e: PcbToastEvent) {
        addToast(e, this);
        this._notificationsElement?.hidePopover?.();
        this._notificationsElement?.showPopover?.();
    }

    // ── Value change notification ─────────────────────────────────────

    private _valueChanged() {
        this.dispatchEvent(new UmbChangeEvent());
    }

    // ── Focus block in preview ────────────────────────────────────────

    private onFocusBlock(e: PcbFocusBlockInPreviewEvent) {
        this.focusedBlockId = e.blockId;
    }

    // ── Block ID helpers ──────────────────────────────────────────────

    private get allBlockIds(): string[] {
        const ids: string[] = [];
        if (this._value.header) ids.push(this._value.header.id);
        ids.push(...this._value.blocks.map(b => b.id));
        return ids;
    }

    private get areAllBlocksOpen(): boolean {
        const allIds = this.allBlockIds;
        if (allIds.length === 0) return false;
        return allIds.every(id => this.openedBlocks.includes(id));
    }

    // ── Block mandatory check ─────────────────────────────────────────

    private _isBlockMandatory(block: PerplexContentBlocksBlock, section: Section): boolean {
        const presets = this.ctx.presets;
        if (!presets) return false;

        if (section === Section.HEADER && presets.header) {
            return (
                presets.header.id === block.presetId &&
                presets.header.definitionId === block.definitionId &&
                presets.header.isMandatory
            );
        }

        if (presets.blocks?.length > 0) {
            const presetItem = presets.blocks.find(
                item => item.id === block.presetId && item.definitionId === block.definitionId,
            );
            return presetItem?.isMandatory ?? false;
        }

        return false;
    }

    // ── Toggle / Copy all ─────────────────────────────────────────────

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
            this.dispatchEvent(new PcbToastEvent('warning', { headline: 'No blocks to copy' }));
            return;
        }

        const copied: CopiedData = {
            header: header ? differentiateBlocks([header])[0] : null,
            blocks: differentiateBlocks(blocks),
        };

        this.ctx.setCopied(copied);
        this._copiedValue = copied;
        this.dispatchEvent(
            new PcbToastEvent('positive', {
                headline: `Copied ${totalCount} block${totalCount > 1 ? 's' : ''} to clipboard`,
            }),
        );
    }

    // ── Add header / block ────────────────────────────────────────────

    addHeader() {
        this._openModal(Section.HEADER);
    }

    addBlock() {
        this._openModal(Section.CONTENT);
    }

    // ── Header CRUD ───────────────────────────────────────────────────

    private _updateHeader(header: PerplexContentBlocksBlock) {
        this._value = { ...this._value, header };
        this._valueChanged();
    }

    private _removeHeader() {
        this._value = { ...this._value, header: null };
        this._valueChanged();
    }

    // ── Block toggling ────────────────────────────────────────────────

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
        this._isDraggingBlock = false;
        this._valueChanged();
    }

    updateBlock(event: PcbBlockUpdatedEvent) {
        if (event.section === Section.HEADER) {
            this._updateHeader(event.block);
            return;
        }

        const idx = this._value.blocks.findIndex(b => b.id === event.block.id);
        if (idx === -1) return;

        const blocks = [...this._value.blocks];
        blocks.splice(idx, 1, event.block);
        this._value = { ...this._value, blocks };
        this._valueChanged();
    }

    removeBlock(id: string) {
        const blocks = this._value.blocks.filter(block => block.id !== id);
        this._value = { ...this._value, blocks };
        this._valueChanged();
    }

    // ── Copy / Paste ──────────────────────────────────────────────────

    onValueCopied(event: PcbValueCopiedEvent) {
        const { blocks, section } = event;
        let copied: CopiedData;
        if (section === Section.HEADER && blocks.length > 0) {
            copied = { header: differentiateBlocks([blocks[0]])[0], blocks: [] };
        } else {
            copied = { header: null, blocks: differentiateBlocks(blocks) };
        }
        this.ctx.setCopied(copied);
        this._copiedValue = copied;
    }

    onValuePasted(event: PcbValuePastedEvent) {
        const { pastedValue, section, desiredIndex } = event;
        if (!pastedValue) return;

        const warnings: string[] = [];
        let headerPasted = false;

        // Create fresh copies with new IDs for each paste operation
        const freshHeader = pastedValue.header ? differentiateBlocks([pastedValue.header])[0] : null;
        const freshBlocks = differentiateBlocks(pastedValue.blocks);

        if (freshHeader) {
            if (this._value.header) {
                warnings.push('Header was ignored because one already exists');
            } else {
                const headerDef = this.ctx.findDefinitionById(freshHeader.definitionId);
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
            for (const warning of warnings) {
                this.dispatchEvent(new PcbToastEvent('warning', { headline: warning }));
            }
        }

        if (headerPasted && pastedValue.blocks.length === 0) {
            this._valueChanged();
        }
    }

    pasteBlock(section: Section) {
        const copied = this._copiedValue;
        if (copied) {
            this.dispatchEvent(new PcbValuePastedEvent(copied, section));
        }
    }

    // ── Open modal ────────────────────────────────────────────────────

    private _openModal = async (section: Section, insertAtIndex?: number) => {
        const returnedValue = await umbOpenModal(this, PCB_ADD_BLOCK_MODAL_TOKEN, {
            data: {
                editorId: this.ctx.editorId,
                groupedDefinitions: this._definitions,
                section,
                insertAtIndex,
            },
        }).catch(() => undefined);

        if (!returnedValue) return;
        this.addBlocks(returnedValue.blocks, returnedValue.section, returnedValue.desiredIndex);
    };

    // ── Add blocks logic ──────────────────────────────────────────────

    addBlocks(blocks: PerplexContentBlocksBlock[], section: Section, desiredIndex: number | null) {
        const allowedBlocks = blocks.filter(b => this.ctx.findDefinitionById(b.definitionId) !== null);

        const skippedCount = blocks.length - allowedBlocks.length;
        if (skippedCount > 0) {
            this.dispatchEvent(
                new PcbToastEvent('warning', {
                    headline: `${skippedCount} block${skippedCount > 1 ? 's were' : ' was'} not allowed on this page and ignored`,
                }),
            );
        }

        if (allowedBlocks.length === 0) return;

        this.openedBlocks = [...this.openedBlocks, ...allowedBlocks.map(b => b.id)];

        if (section === Section.HEADER && allowedBlocks.length > 0) {
            const definition = this.ctx.findDefinitionById(allowedBlocks[0].definitionId);
            if (definition?.categoryIds.some(id => this.ctx.headerCategories.includes(id))) {
                this._value = { ...this._value, header: allowedBlocks[0] };
            } else {
                this.dispatchEvent(
                    new PcbToastEvent('warning', { headline: 'This block cannot be added as a header' }),
                );
            }
        } else {
            const contentBlocks = allowedBlocks.filter(b => {
                const def = this.ctx.findDefinitionById(b.definitionId);
                if (!def) return false;
                return !def.categoryIds.every(id => this.ctx.headerCategories.includes(id));
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

        this._valueChanged();
    }

    // ── Render ─────────────────────────────────────────────────────────

    render() {
        const header = this._value.header;
        const headerDefinition = header ? this.ctx.findDefinitionById(header.definitionId) : null;
        const blocks = this._value.blocks
            .map(block => ({ block, definition: this.ctx.findDefinitionById(block.definitionId) }))
            .filter(item => item.definition !== null);

        const hasCopied = this._copiedValue != null;

        return html`
            <section class="section-controls">
                ${this.config?.getValueByAlias('hideControls') !== true
                    ? html` <div class="controls-bar">
                          <uui-button
                              look="secondary"
                              @click=${this.toggleAllBlocks}
                          >
                              <slot name="extra">
                                  <uui-icon
                                      name=${this.areAllBlocksOpen ? 'icon-defrag' : 'icon-fullscreen-alt'}
                                  ></uui-icon>
                              </slot>
                              <slot name="label"
                                  >${this.areAllBlocksOpen ? 'Close all blocks' : 'Open all blocks'}</slot
                              >
                          </uui-button>
                          <uui-button
                              look="secondary"
                              @click=${this.copyAllBlocks}
                          >
                              <slot name="extra">
                                  <uui-icon name="icon-documents"></uui-icon>
                              </slot>
                              <slot name="label">Copy all blocks</slot>
                          </uui-button>
                      </div>`
                    : nothing}
            </section>
            <section class="section-main">
                <div class="main">
                    <div class="pcb__wrapper">
                        <div class="pcb__content">
                            <div class="pcb__blocks">
                                ${header && headerDefinition && this.structure !== Structure.Blocks
                                    ? html`
                                          <pcb-block
                                              .draggable=${false}
                                              .block=${header}
                                              .collapsed=${!this.openedBlocks.includes(header.id)}
                                              .removeBlock=${this._removeHeader.bind(this)}
                                              .dataPath=${this.dataPath}
                                              .definition=${headerDefinition}
                                              .section=${Section.HEADER}
                                              .openModal=${this._openModal}
                                              .isDraggingBlock=${this._isDraggingBlock}
                                              .isMandatory=${this._isBlockMandatory(header, Section.HEADER)}
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

                                              ${hasCopied
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
                                                  blocks,
                                                  ({ block }) => block.id,
                                                  ({ block, definition }, index) => html`
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
                                                              .definition=${definition!}
                                                              .section=${Section.CONTENT}
                                                              .index=${index}
                                                              .openModal=${this._openModal}
                                                              .isDraggingBlock=${this._isDraggingBlock}
                                                              .isMandatory=${this._isBlockMandatory(
                                                                  block,
                                                                  Section.CONTENT,
                                                              )}
                                                              .hasCopiedValue=${hasCopied}
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

                                          ${hasCopied
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
                                  ${this.showDebug
                                      ? html`
                                            <pre style="white-space:pre-wrap;font-size:90%">
                                            ${JSON.stringify(this.value, null, 4)}
                                        </pre
                                            >
                                        `
                                      : nothing}
                              </div>
                          `
                        : nothing}
                </div>
                ${this.config?.getValueByAlias('hideControls') !== true ||
                (this.pageId && this.config?.getValueByAlias('hidePreview') !== true)
                    ? html`
                          <div class="sidebar">
                              ${this.pageId && this.config?.getValueByAlias('hidePreview') !== true
                                  ? html`<div class="sidebar__section">
                                        <pcb-preview
                                            .culture=${this.culture}
                                            .focusedBlockId=${this.focusedBlockId}
                                            .pageId=${this.pageId}
                                        ></pcb-preview>
                                    </div>`
                                  : nothing}
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
            </section>
        `;
    }

    static styles = [
        css`
            .section-controls {
                display: flex;
                justify-content: flex-end;
            }

            .section-main {
                display: grid;
                gap: 0.8rem;
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
                background: none;
                outline: 0;
                border: 0;
                margin: 0;
            }

            .main {
                box-sizing: border-box;
            }

            .main,
            .sidebar,
            .controls-bar {
                padding: 0.5rem 0.9rem;
            }

            .sidebar {
                display: none;
                flex-direction: column;
                gap: var(--uui-size-4);
                position: sticky;
                top: 0;

                @media only screen and (min-width: 1800px) {
                    display: flex;
                }
            }

            .sidebar__section {
                background-color: var(--uui-color-surface);
                border: 1px solid var(--uui-color-border);
                border-radius: var(--uui-border-radius);
            }

            .controls-bar {
                display: flex;
                gap: var(--uui-size-3);
                padding-top: 0;
            }

            .pcb__wrapper {
                display: block;
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
                    gap: var(--uui-size-4);

                    &.pcb__block-add--header {
                        margin-block-end: 1rem;
                    }
                }
            }
        `,
    ];
}
