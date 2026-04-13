import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { customElement, html, nothing, property, unsafeCSS } from '@umbraco-cms/backoffice/external/lit';

import blockSpacerStyles from './pcb-block-spacer.css?inline';
import { Section } from '../../../types.ts';
import { PcbValuePastedEvent } from '../../../events/copyPaste.ts';
import { consume } from '@lit/context';
import { pcbEditorContext } from '../../../context';
import { PcbEditorContext } from '../../../context/pcb-editor-context.ts';

@customElement('pcb-block-spacer')
export default class PerplexContentBlocksBlockSpacerElement extends UmbLitElement {
    @property({ type: Number, attribute: 'index' })
    index: number = 0;

    @property({ attribute: false })
    openModal!: (section: Section, insertAtIndex: number) => any;

    @property({ type: Boolean })
    hasCopiedValue: boolean = false;

    @consume({ context: pcbEditorContext })
    ctx!: PcbEditorContext;

    addBlock() {
        this.openModal(Section.CONTENT, this.index);
    }

    pasteBlock() {
        const copied = this.ctx.getCopied();
        if (copied) {
            this.dispatchEvent(new PcbValuePastedEvent(copied, Section.CONTENT, this.index));
        }
    }

    render() {
        return html`<div class="pcb-block-spacer">
            <div>
                <div class="pcb-block-spacer__controls">
                    <uui-button
                        label="add block"
                        look="primary"
                        @click=${this.addBlock}
                    >
                        <slot name="label"> Add content </slot>

                        <slot name="extra">
                            <uui-icon name="icon-add"></uui-icon>
                        </slot>
                    </uui-button>

                    ${this.hasCopiedValue
                        ? html` <uui-button
                              label="paste content"
                              look="primary"
                              @click=${this.pasteBlock}
                          >
                              <slot name="label"> Paste content </slot>

                              <slot name="extra">
                                  <uui-icon name="icon-clipboard-paste"></uui-icon>
                              </slot>
                          </uui-button>`
                        : nothing}
                </div>
            </div>
        </div>`;
    }

    static styles = [unsafeCSS(blockSpacerStyles)];
}
