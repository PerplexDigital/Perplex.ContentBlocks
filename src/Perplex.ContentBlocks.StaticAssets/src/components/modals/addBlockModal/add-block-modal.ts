import { customElement, html, query, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { connect } from 'pwa-helpers';
import { store } from '../../../state/store.ts';
import { LitElement, unsafeCSS } from 'lit';
import { resetAddBlockModal, setAddBlockModal } from '../../../state/slices/ui.ts';
import { PCBCategoryWithDefinitions, PerplexContentBlocksBlock, Section } from '../../../types.ts';
import { BlockSavedEvent, ON_BLOCK_SELECTED } from '../../../events/block.ts';
import { ToastEvent } from '../../../events/toast.ts';
import addBlockModalStyles from './addBlockModal.css?inline';

@customElement('pcb-add-block-modal')
export default class PcbAddBlockModal extends connect(store)(LitElement) {
    @query('#popover-container')
    private _notificationsElement?: HTMLElement;

    @state()
    groupedDefinitions?: PCBCategoryWithDefinitions[];

    @state()
    selectedBlock: PerplexContentBlocksBlock | null = null;

    @state()
    section: Section = Section.CONTENT;

    @state()
    insertAtIndex?: number;

    @state()
    searchTerm: string | null = null;

    @state()
    selectedCategories: string[] | null = null;

    connectedCallback() {
        super.connectedCallback();
        this.addEventListener(ON_BLOCK_SELECTED, (e: Event) => this.onBlockSelected(e as CustomEvent));
    }

    onBlockSelected(event: CustomEvent) {
        this.selectedBlock = event.detail;
    }

    stateChanged(state: any) {
        this.groupedDefinitions = state.definitions.value;

        if (state.ui.addBlock.display) {
            this._notificationsElement?.showPopover?.();
        } else {
            this._notificationsElement?.hidePopover?.();
        }

        this.section = state.ui.addBlock.section;

        this.insertAtIndex = state.ui.addBlock.insertAtIndex;
    }

    onPopoverClick() {
        store.dispatch(resetAddBlockModal());
        this._notificationsElement?.hidePopover?.();
    }

    onSaveClicked() {
        if (!this.selectedBlock) {
            this.dispatchEvent(
                ToastEvent('warning', {
                    headline: 'Select a content block',
                }),
            );
            return;
        }

        this.dispatchEvent(
            BlockSavedEvent({
                blocks: [this.selectedBlock],
                section: this.section,
                desiredIndex: this.insertAtIndex ?? null,
            }),
        );
        this.selectedBlock = null;
    }

    onSearchTermChanged(e: Event) {
        const input = e.target as HTMLInputElement;
        this.searchTerm = input.value;
    }

    onCategoryClicked(e: Event, categoryId: string) {
        e.preventDefault();
        if (!this.selectedCategories) {
            this.selectedCategories = [];
        }

        if (this.selectedCategories.includes(categoryId)) {
            this.selectedCategories = this.selectedCategories.filter((id) => id !== categoryId);
        } else {
            this.selectedCategories = [...this.selectedCategories, categoryId];
        }
    }

    onResetFilters() {
        this.selectedCategories = null;
        this.searchTerm = null;
    }

    renderBlocks() {
        if (!this.groupedDefinitions) return;

        const searchTerm = this.searchTerm?.toLowerCase().trim() || '';

        // Only show categories that match the section.
        const categories = Object.values(this.groupedDefinitions).filter((category) => {
            const isHeader = this.section === Section.HEADER;
            return category.category.isEnabledForHeaders === isHeader;
        });

        const filteredCategories = categories
            .map((category) => {
                // Filter definitions by search term
                const filteredDefinitions = Object.values(category.definitions).filter((definition) =>
                    `${definition!.name} | ${definition.layouts.map((layout) => layout.name).join(' | ')}`
                        .toLowerCase()
                        .includes(searchTerm),
                );

                // If categories are selected, filter by selected categories
                if (this.selectedCategories && this.selectedCategories.length > 0) {
                    if (!this.selectedCategories.includes(category.category.id)) {
                        return { ...category, filteredDefinitions: [] };
                    }
                }
                return { ...category, filteredDefinitions };
            })
            // Exclude categories with no matching definitions
            .filter((category) => category.filteredDefinitions.length > 0);

        return html`
            <ul class="addBlockModal__blockList">
                <div class="addBlockModal__filterBar">
                    <div class="addBlockModal__searchBar">
                        <uui-input
                            @input=${this.onSearchTermChanged}
                            placeholder="Search blocks..."
                            .value=${this.searchTerm || ''}
                        ></uui-input>
                        <uui-button
                            look="primary"
                            ?disabled=${!this.searchTerm && !this.selectedCategories}
                            @click=${this.onResetFilters}
                        >
                            Reset <uui-icon name="icon-trash"></uui-icon>
                        </uui-button>
                    </div>
                    <div class="addBlockModal__filters">
                        ${categories.map((category) => {
                            return html`
                                <label
                                    class="addBlockModal__filter"
                                    for=${category.category.name}
                                >
                                    <span> ${category.category.name} </span>
                                    <input
                                        id=${category.category.name}
                                        type="checkbox"
                                        name="category"
                                        @change=${(e: Event) => this.onCategoryClicked(e, category.category.id)}
                                        .checked=${this.selectedCategories?.includes(category.category.id) ?? false}
                                    />
                                </label>
                            `;
                        })}
                    </div>
                </div>

                ${filteredCategories.length === 0
                    ? html`<div class="addBlockModal__noResults">No blocks found</div>`
                    : repeat(
                          filteredCategories,
                          (category) => category.category.id,
                          (category) => html`
                              ${repeat(
                                  category.filteredDefinitions,
                                  (definition) => definition.id,
                                  (definition) => html`
                                      <pcb-block-definition
                                          selected=${definition.id === this.selectedBlock?.definitionId}
                                          .definition=${definition}
                                      ></pcb-block-definition>
                                  `,
                              )}
                          `,
                      )}
            </ul>
        `;
    }

    render() {
        if (!this.groupedDefinitions) return;
        return html`
            <div
                id="popover-container"
                popover
                @click=${this.onPopoverClick}
            >
                <div
                    class="addBlockModal"
                    @click=${(e: Event) => e.stopPropagation()}
                >
                    ${this.renderBlocks()}

                    <div class="addBlockModal__sidebarButtons">
                        <uui-button
                            look="secondary"
                            type="button"
                            @click=${() => store.dispatch(setAddBlockModal(false))}
                        >
                            Close
                        </uui-button>

                        <uui-button
                            look="primary"
                            type="button"
                            ?disabled=${!this.selectedBlock}
                            @click=${this.onSaveClicked}
                        >
                            Save
                        </uui-button>
                    </div>
                </div>
            </div>
        `;
    }

    static styles = [unsafeCSS(addBlockModalStyles)];
}
