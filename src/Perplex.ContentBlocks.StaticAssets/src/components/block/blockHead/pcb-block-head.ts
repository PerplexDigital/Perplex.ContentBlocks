import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
    html,
    customElement,
    property,
    unsafeCSS,
    nothing,
    state,
    query,
    PropertyValues,
} from '@umbraco-cms/backoffice/external/lit';
import { PcbBlockToggleEvent, PcbBlockUpdatedEvent, ON_BLOCK_REMOVE } from '../../../events/block.ts';
import blockHeadStyles from './block-head.css?inline';
import baseStyles from './../../../css/base.css?inline';
import { PerplexBlockDefinition, PerplexContentBlocksBlock, Section } from '../../../types.ts';
import { PcbValueCopiedEvent } from '../../../events/copyPaste.ts';
import { PcbToastEvent } from '../../../events/toast.ts';
import { consume } from '@lit/context';
import { pcbEditorContext } from '../../../context';
import { PcbEditorContext } from '../../../context/pcb-editor-context.ts';

const OLD_SYNTAX_SINGLE_VALUE = /^\{\{\s*(\w+)\s*\}\}$/;

/** Text content of a node including text rendered inside nested shadow roots. */
function getText(node: Node): string {
    if (node.nodeType === Node.TEXT_NODE) return node.textContent ?? '';
    if (node instanceof HTMLStyleElement || node instanceof HTMLScriptElement) return '';

    let text = '';
    if (node instanceof Element && node.shadowRoot) {
        // Only the shadow root is rendered; light DOM children (e.g. the raw expression
        // inside <umb-ufm-js-expression>) are not displayed since UFM components have no slots.
        return getText(node.shadowRoot);
    }
    node.childNodes.forEach(child => (text += getText(child)));
    return text;
}

@customElement('pcb-block-head')
export default class PcbBlockHead extends UmbLitElement {
    @property({ attribute: false })
    definition: PerplexBlockDefinition | null = null;

    @property({ attribute: false })
    blockDefinitionName!: string;

    @property({ attribute: false })
    get blockNameTemplate(): string | undefined {
        return this._blockNameTemplate;
    }

    set blockNameTemplate(value: string | undefined) {
        if (value != null) {
            const match = value.match(OLD_SYNTAX_SINGLE_VALUE);
            if (match) {
                // For backwards compatibility we will transform the old {{ALIAS}} syntax to the new {umbValue: ALIAS} syntax
                // but only when the entire template was just a single value, e.g. "{{title}}".
                value = `{umbValue: ${match[1]}}`;
            }
        }

        this._blockNameTemplate = value;
    }

    private _blockNameTemplate?: string;

    @property({ attribute: false })
    collapsed: boolean = false;

    @property({ attribute: false })
    id!: string;

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

    @state()
    private blockValuesByAlias: Record<string, any> = {};

    @property({ attribute: false })
    section: Section = Section.CONTENT;

    @property({ type: Boolean })
    isDraggingBlock: boolean = false;

    @property({ type: Boolean })
    isMandatory!: boolean;

    @state()
    selectedLayoutIndex: number = 0;

    @query('#tooltip-popover')
    private _tooltipPopover!: HTMLElement;

    @consume({ context: pcbEditorContext })
    ctx!: PcbEditorContext;

    @query('#tooltip-header-block')
    private _tooltipHeaderBlock!: HTMLElement;

    private crossedEyeIcon(fontSize = '20px') {
        return html`
            <uui-icon style="font-size: ${fontSize};">
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="22"
                    height="22"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    class="lucide lucide-eye-off-icon lucide-eye-off"
                >
                    <path
                        d="M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49"
                    />
                    <path d="M14.084 14.158a3 3 0 0 1-4.242-4.242" />
                    <path
                        d="M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143"
                    />
                    <path d="m2 2 20 20" />
                </svg>
            </uui-icon>
        `;
    }

    #tooltipOnMouseEnter() {
        if (!this.collapsed) {
            this._tooltipPopover.showPopover();
        }

        if (this.collapsed && this.section === Section.HEADER) {
            this._tooltipHeaderBlock.showPopover();
        }
    }

    #tooltipOnMouseLeave() {
        this._tooltipPopover.hidePopover();
        this._tooltipHeaderBlock?.hidePopover();
    }

    onHeadClicked = () => {
        if (this.definition == null) return;
        this.dispatchEvent(new PcbBlockToggleEvent(this.id));
    };

    onRemoveClicked = (e: Event) => {
        this.dispatchEvent(new CustomEvent(ON_BLOCK_REMOVE, e));
    };

    onToggleVisibilityClicked = () => {
        const updatedBlock = { ...this.block, isDisabled: !this.block.isDisabled };
        this.dispatchEvent(
            new PcbBlockUpdatedEvent(updatedBlock, this.definition ?? ({} as PerplexBlockDefinition), this.section),
        );
    };

    onCopyClicked = () => {
        if (this.definition == null) return;

        this.dispatchEvent(new PcbValueCopiedEvent([this.block], this.section));
        this.dispatchEvent(
            new PcbToastEvent('positive', {
                headline: 'Copied!',
                message: `${this.definition.name} copied to clipboard`,
            }),
        );
    };

    /**
     * Whether the block name template, as rendered by UFM, produced any visible text.
     * We cannot know this up front (UFM expressions and components like {umbValue: ...}
     * resolve asynchronously, partly in nested shadow roots), so we observe the rendered
     * DOM of <umb-ufm-render> and read its trimmed text content.
     */
    @state()
    private hasBlockNameValue = false;

    @query('umb-ufm-render')
    private _ufmRender?: HTMLElement;

    #observedUfmRender?: HTMLElement;
    #ufmObservers = new Map<Node, MutationObserver>();

    #observeUfmRender() {
        const el = this._ufmRender;
        if (el === this.#observedUfmRender) return;

        this.#disconnectUfmObservers();
        this.#observedUfmRender = el;

        if (el?.shadowRoot) {
            this.#observeNode(el.shadowRoot);
        }

        this.#updateHasBlockNameValue();
    }

    #updateScheduled = false;

    #observeNode(root: Node) {
        if (this.#ufmObservers.has(root)) return;

        const observer = new MutationObserver(mutations => this.#onUfmMutation(mutations));
        observer.observe(root, { childList: true, subtree: true, characterData: true });
        this.#ufmObservers.set(root, observer);
        this.#discoverShadowRoots(root);
    }

    #onUfmMutation(mutations: MutationRecord[]) {
        // Nested custom elements (e.g. umb-ufm-js-expression) render inside their own shadow roots,
        // which are invisible to a parent observer. Only inspect added/removed nodes.
        for (const mutation of mutations) {
            mutation.addedNodes.forEach(node => {
                if (!(node instanceof Element)) return;
                if (node.shadowRoot) this.#observeNode(node.shadowRoot);
                this.#discoverShadowRoots(node);
            });
            mutation.removedNodes.forEach(node => {
                if (node instanceof Element) this.#disposeShadowObservers(node);
            });
        }
        this.#scheduleUpdateHasBlockNameValue();
    }

    #discoverShadowRoots(root: Node) {
        root.childNodes.forEach(child => {
            if (child instanceof Element && child.shadowRoot) {
                this.#observeNode(child.shadowRoot);
            }
            this.#discoverShadowRoots(child);
        });
    }

    #disposeShadowObservers(element: Element) {
        if (element.shadowRoot) {
            this.#ufmObservers.get(element.shadowRoot)?.disconnect();
            this.#ufmObservers.delete(element.shadowRoot);
            this.#disposeShadowObserversIn(element.shadowRoot);
        }
        this.#disposeShadowObserversIn(element);
    }

    #disposeShadowObserversIn(root: Node) {
        root.childNodes.forEach(child => {
            if (child instanceof Element) this.#disposeShadowObservers(child);
        });
    }

    #disconnectUfmObservers() {
        this.#ufmObservers.forEach(o => o.disconnect());
        this.#ufmObservers.clear();
        this.#observedUfmRender = undefined;
    }

    /** Batches recalculation so multiple observers firing in the same tick compute the text once. */
    #scheduleUpdateHasBlockNameValue() {
        if (this.#updateScheduled) return;
        this.#updateScheduled = true;
        queueMicrotask(() => {
            this.#updateScheduled = false;
            this.#updateHasBlockNameValue();
        });
    }

    #updateHasBlockNameValue() {
        const root = this.#observedUfmRender?.shadowRoot;
        const hasValue = root != null && getText(root).trim().length > 0;
        if (hasValue !== this.hasBlockNameValue) {
            this.hasBlockNameValue = hasValue;
        }
    }

    protected updated(_changedProperties: PropertyValues<this>) {
        super.updated(_changedProperties);
        this.#observeUfmRender();
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.#disconnectUfmObservers();
    }

    protected willUpdate(_changedProperties: PropertyValues<this>) {
        if (_changedProperties.has('definition') || _changedProperties.has('block')) {
            this.selectedLayoutIndex = this.definition?.layouts.findIndex(l => l.id === this.block.layoutId) || 0;
        }

        if (_changedProperties.has('block')) {
            this.blockValuesByAlias = {};

            if (Array.isArray(this.block?.content?.values)) {
                for (const value of this.block.content.values) {
                    this.blockValuesByAlias[value.alias] = value.value;
                }
            }
        }
    }

    render() {
        const isTouchDevice = this.ctx?.isTouchDevice ?? false;

        return html`
            <div
                class="block-head ${this.block.isDisabled ? 'block-head--disabled' : ''} ${this.definition == null
                    ? 'block-head--missing-definition'
                    : ''}"
            >
                <button
                    type="button"
                    @click=${this.onHeadClicked}
                    class=${`block-head__toggle ${this.collapsed ? '' : 'block-head--open'}`}
                    aria-expanded=${!this.collapsed}
                    aria-label="Toggle block ${this.blockDefinitionName}"
                >
                    ${!isTouchDevice
                        ? html`
                              <div
                                  id="tooltip-toggle"
                                  class="block-head__handle-wrapper"
                                  popovertarget=${this.collapsed && this.section === Section.HEADER
                                      ? 'tooltip-header-block'
                                      : 'tooltip-popover'}
                                  @mouseenter=${this.#tooltipOnMouseEnter}
                                  @mouseleave=${this.#tooltipOnMouseLeave}
                              >
                                  ${this.section === Section.CONTENT
                                      ? html`
                                            <uui-icon
                                                class="block-head__handle icon icon--base"
                                                name="icon-grip"
                                            ></uui-icon>
                                        `
                                      : nothing}
                              </div>
                              <uui-popover-container id="tooltip-popover">
                                  <div
                                      style="font-size: var(--uui-type-small-size); color: var(--uui-color-surface); max-width: 320px; padding: var(--uui-size-space-4); background-color: var(--uui-color-text); border-radius: var(--uui-border-radius); box-shadow: var(--uui-shadow-depth-4);"
                                  >
                                      An expanded block cannot be dragged. Collapse the block to drag it.
                                  </div>
                              </uui-popover-container>
                              ${this.section === Section.HEADER
                                  ? html`
                                        <uui-popover-container id="tooltip-header-block">
                                            <div
                                                style="font-size: var(--uui-type-small-size); color: var(--uui-color-surface); max-width: 320px; padding: var(--uui-size-space-4); background-color: var(--uui-color-text); border-radius: var(--uui-border-radius); box-shadow: var(--uui-shadow-depth-4);"
                                            >
                                                A header block cannot be dragged because it should always be positioned
                                                at the top op the page.
                                            </div>
                                        </uui-popover-container>
                                    `
                                  : nothing}
                          `
                        : nothing}
                    <div class="block-head__title">
                        <strong>
                            ${this.definition == null
                                ? 'Block definition not found'
                                : this.blockNameTemplate
                                  ? html`<umb-ufm-render
                                        inline
                                        style=${this.hasBlockNameValue ? '' : 'display: none'}
                                        .markdown=${this.blockNameTemplate}
                                        .value=${this.blockValuesByAlias}
                                    ></umb-ufm-render>`
                                  : nothing}
                        </strong>
                        ${this.block.isDisabled
                            ? html`
                                  <uui-tag style="--uui-tag-border-radius: 30px;">
                                      ${this.crossedEyeIcon('12px')}
                                      <span>Hidden</span>
                                  </uui-tag>
                              `
                            : nothing}
                        <div
                            class="${`block-head__description ${
                                this.definition != null && !this.hasBlockNameValue
                                    ? 'block-head__description--no-title'
                                    : ''
                            }`}"
                        >
                            ${this.definition == null ? this.block.definitionId : this.blockDefinitionName}
                        </div>
                    </div>
                </button>
                ${this.definition != null && !this.isDraggingBlock
                    ? html`
                          <pcb-inline-layout-switch
                              .definition=${this.definition}
                              .initialSlideIndex=${this.selectedLayoutIndex}
                          ></pcb-inline-layout-switch>
                      `
                    : nothing}
                <div class="block-head__controls">
                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onToggleVisibilityClicked}
                        aria-label=${this.block.isDisabled ? 'Show block' : 'Hide block'}
                    >
                        ${this.block.isDisabled
                            ? this.crossedEyeIcon()
                            : html`
                                  <uui-icon
                                      style="font-size: 20px;"
                                      name="icon-eye"
                                  >
                                  </uui-icon>
                              `}
                    </button>
                    ${this.definition != null
                        ? html` <button
                              class="block-head__control"
                              type="button"
                              @click=${this.onCopyClicked}
                              ?disabled=${this.definition == null}
                              aria-label="Copy block"
                          >
                              <uui-icon
                                  style="font-size: 20px;"
                                  name="icon-documents"
                              >
                              </uui-icon>
                          </button>`
                        : nothing}
                    <button
                        class="block-head__control"
                        type="button"
                        @click=${this.onRemoveClicked}
                        ?disabled=${this.isMandatory}
                        aria-label="Remove block"
                    >
                        <uui-icon
                            style="font-size: 20px;"
                            name="icon-trash"
                        >
                        </uui-icon>
                    </button>
                </div>
            </div>
        `;
    }

    static styles = [unsafeCSS(blockHeadStyles), unsafeCSS(baseStyles)];
}
