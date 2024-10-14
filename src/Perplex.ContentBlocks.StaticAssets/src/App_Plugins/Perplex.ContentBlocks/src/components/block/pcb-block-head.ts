import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import {html, customElement, property, css, nothing} from "@umbraco-cms/backoffice/external/lit";
import {variables} from "../../styles.ts";
import {BlockToggleEvent, ON_BLOCK_REMOVE} from "../../events/block.ts";

@customElement("pcb-block-head")
export default class PcbBlockHead extends UmbLitElement {
    @property({ attribute: false })
    icon!: string;

    @property({ attribute: false })
    blockDefinitionName!: string;

    @property({ attribute: false })
    blockTemplateName?: string;

    @property({ attribute: false })
    collapsed: boolean = false;

    @property({ attribute: false })
    id!: string;


    onHeadClicked = () => {
        this.dispatchEvent(BlockToggleEvent(this.id));
    }

    onRemoveClicked = (e: Event) => {
        this.dispatchEvent(new CustomEvent(ON_BLOCK_REMOVE, e))
    }

    render() {
        return html`
            <div class="block-head">
                <button
                        type="button"
                        @click=${this.onHeadClicked}
                        class=${`block-head__toggle ${this.collapsed ? '' : 'block-head--open'}`}
                >
                    <uui-icon
                            style="font-size: 20px; color: var(--c-submarine);"
                            .name=${this.icon}
                    >
                    </uui-icon>
                    <div class="block-head__title">
                        ${this.blockTemplateName ? html`<strong>${this.blockTemplateName}</strong>` : nothing}
                        <div>${this.blockDefinitionName}</div>
                    </div>
                </button>
                <button
                    class="block-head__control"
                    type="button"
                    @click=${this.onRemoveClicked}
                >
                    <uui-icon
                            style="font-size: 20px; color: var(--c-submarine);"
                            name="icon-trash"
                    >
                    </uui-icon>
                </button>
            </div>

        `;
    }

    static styles = [
        variables,
        css`
            .block-head {
                background-color: var(--c-mystic);
                border-radius: var(--r-lg);
                display: flex;
                align-items: stretch;
                width: 100%;
                border: none;
                cursor: pointer;
                border-bottom: 1px solid rgba(var(--c-submarine), 0);
                transition: 250ms border ease;

                .block-head__control {
                    border: none;
                    cursor: pointer;
                    &:hover {
                        color: var(--c-danger);
                    }
                }

                .block-head__toggle {
                    display: flex;
                    align-items: stretch;
                    width: 100%;
                    border: none;
                    cursor: pointer;
                    transition: 250ms border ease;
                }

                &.block-head--open {
                    border-radius: var(--r-lg) 0 border-radius: var(--r-lg) 0;
                    border-bottom: 1px solid rgba(var(--c-submarine), 1);
                }


                uui-icon {
                    padding: calc(var(--s) * 4);
                }

                .block-head__title {
                    display: flex;
                    min-width: 300px;
                    align-items: start;
                    justify-content: center;
                    padding: 0 calc(var(--s) * 6);
                    flex-direction: column;
                    border-left: 1px solid rgb(var(--c-submarine));
                    border-right: 1px solid rgb(var(--c-submarine));
                    flex: 1;

                    div {
                        font-size: var(--fs-sm);
                    }

                    strong {
                        font-size: var(--fs-base);
                    }
                }
            }
        `,
    ];
}
