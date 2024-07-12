import {html, customElement, state, nothing, repeat, css, query} from "@umbraco-cms/backoffice/external/lit";
import {connect} from "pwa-helpers";
import {store} from "../../state/store.ts";
import {LitElement} from "lit";
import {DefinitionsDictionary} from "../../state/slices/definitions.ts";
import {variables} from "../../styles.ts";
import {setAddBlockModal} from "../../state/slices/ui.ts";
import {PerplexContentBlocksBlock} from "../../types.ts";
import {BlockSavedEvent, ON_BLOCK_SELECTED} from "../../events/block.ts";
import {ToastEvent} from "../../events/toast.ts";

@customElement("pcb-add-block-modal")
export default class PcbAddBlockModal extends connect(store)(LitElement) {
    @query('#popover-container')
    private _notificationsElement?: HTMLElement;

    @state()
    displayAddBlockModal = false;

    @state()
    definitions?: DefinitionsDictionary

    @state()
    selectedBlock: PerplexContentBlocksBlock | null = null;

    onClick() {
        this._notificationsElement?.togglePopover?.();
    }

    connectedCallback() {
        super.connectedCallback();
        this.addEventListener(ON_BLOCK_SELECTED, (e: Event ) =>this.onBlockSelected(e as CustomEvent))
    }

    onBlockSelected(event: CustomEvent) {
        this.selectedBlock = event.detail;
    }

    stateChanged(state: any) {
        this.definitions = state.definitions.value;

        // Show popover when ui.displayAddBlockModal is true.
        if (state.ui.displayAddBlockModal) {
            this._notificationsElement?.showPopover?.();
        } else {
            this._notificationsElement?.hidePopover?.();
        }
    }

    onPopoverClick() {
        this._notificationsElement?.hidePopover?.()
    }

    onSaveClicked() {
        if (!this.selectedBlock) {
            this.dispatchEvent(ToastEvent('warning', {
                headline: 'Select a content block'
            }))
            return;
        }

        this.dispatchEvent(BlockSavedEvent(this.selectedBlock));
    }

    render() {
        if (!this.definitions) return;
        return html`
           <div id="popover-container" popover @click=${this.onPopoverClick}>
               <div class="addBlockModal" @click=${(e: Event) => e.stopPropagation()}>
                   <ul class="addBlockModal__blockList">
                       ${repeat(
                               Object.values(this.definitions),
                               property => property.id,
                               property =>
                                   html`
                                        <pcb-block-definition
                                            selected=${property.id === this.selectedBlock?.definitionId}
                                            .definition=${property}
                                        ></pcb-block-definition>
                                   `
                       )}
                   </ul>

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
                               @click=${this.onSaveClicked}
                       >
                           Save
                       </uui-button>
                   </div>
               </div>
           </div>
        `;
    }

    static styles = [
        variables,
        css`
            :host {
                --slider-width: 95vw;

                @media only screen and (min-width: 1200px) {
                    --slider-width: 70vw;
                }
            }



            [popover] {
                animation: fadeIn 0.1s ease-in;
                z-index: 5;
            }

            [popover] .addBlockModal {
                animation: testSlideIn 0.3s ease-out forwards;
            }

            @keyframes fadeIn {
                from {
                    opacity: 0;
                }
                to {
                    opacity: 1;
                }
            }

            @keyframes testSlideIn {
                from {
                    right: calc(var(--slider-width) * -1);
                }
                to {
                    right: 0;
                }
            }

            #popover-container {
                overflow: hidden;
                position: fixed;
                height: 100vh;
                width: 100vw;
                background-color: rgba(0, 0, 0, 0.5);
            }

            .addBlockModal {
                background-color: var(--uui-color-background);
                position: absolute;
                top:0;
                right: calc(var(--slider-width) * -1);
                transition: right var(--uui-modal-transition-duration, 250ms);
                display: flex;
                justify-content: space-between;
                flex-direction: column;
                height: 100%;
                width: var(--slider-width);

                @media only screen and (min-width: 1200px) {
                    grid-template-columns: repeat(4, 1fr);
                }

                .addBlockModal__blockList {
                    display: grid;
                    grid-template-columns: repeat(1, 1fr);
                    gap:20px;
                    padding: calc(var(--s) * 20);

                    @media only screen and (min-width: 500px) {
                        grid-template-columns: repeat(2, 1fr);
                    }

                    @media only screen and (min-width: 900px) {
                        grid-template-columns: repeat(3, 1fr);
                    }

                    @media only screen and (min-width: 1400px) {
                        grid-template-columns: repeat(4, 1fr);
                    }

                }

                .addBlockModal__sidebarButtons {
                    margin-top: auto;
                    display: flex;
                    gap: .75rem;
                    align-items: center;
                    justify-content: end;
                    padding: 1rem 2rem;
                    background: var(--uui-color-surface);
                    box-shadow: var(--uui-shadow-depth-4);
                }
            }

        `
    ];
}
