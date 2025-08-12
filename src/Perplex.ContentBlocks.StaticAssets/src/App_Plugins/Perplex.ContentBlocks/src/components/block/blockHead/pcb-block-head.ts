import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import {html, customElement, property, unsafeCSS, nothing, state} from "@umbraco-cms/backoffice/external/lit";
import {BlockToggleEvent, ON_BLOCK_REMOVE} from "../../../events/block.ts";
import styles from "./block-head.css?inline";
import {PerplexBlockDefinition, PerplexContentBlocksBlock} from '../../../types.ts';
import {PropertyValues} from "lit";

@customElement("pcb-block-head")
export default class PcbBlockHead extends UmbLitElement {
    @property()
    definition?: PerplexBlockDefinition;

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

    @property({ attribute: false })
    block!: PerplexContentBlocksBlock;

    @state()
    selectedLayoutIndex: number = 0;


    onHeadClicked = () => {
        this.dispatchEvent(BlockToggleEvent(this.id));
    }

    onRemoveClicked = (e: Event) => {
        this.dispatchEvent(new CustomEvent(ON_BLOCK_REMOVE, e))
    }

    protected willUpdate(_changedProperties: PropertyValues) {
        if (_changedProperties.has('definition') || _changedProperties.has('block')) {
            this.selectedLayoutIndex =
                this.definition?.layouts.findIndex((l) => l.id === this.block.layoutId) || 0;
        }
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
                <pcb-inline-layout-switch
                    .definition=${this.definition}
                    .initialSlideIndex=${this.selectedLayoutIndex}
                ></pcb-inline-layout-switch>
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
        unsafeCSS(styles)
    ];
}
