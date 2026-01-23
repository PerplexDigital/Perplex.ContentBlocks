import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { customElement, html, nothing, property, state, unsafeCSS } from '@umbraco-cms/backoffice/external/lit';

import blockSpacerStyles from './pcb-block-spacer.css?inline';
import { connect } from 'pwa-helpers';
import { store } from '../../../state/store.ts';
import { Section } from '../../../types.ts';
import { PcbValuePastedEvent } from '../../../events/copyPaste.ts';
import { CopyPasteState } from '../../../state/slices/copyPaste.ts';

@customElement('pcb-block-spacer')
export default class PerplexContentBlocksBlockSpacerElement extends connect(store)(UmbLitElement) {
    @property({ type: Number, attribute: 'index' })
    index: number = 0;

    @property({ attribute: false })
    openModal!: (section: Section, insertAtIndex: number) => any;

    @state()
    copiedValue?: CopyPasteState;

    stateChanged(state: any) {
        this.copiedValue = state.copyPaste;
    }

    addBlock() {
        this.openModal(Section.CONTENT, this.index);
    }

    pasteBlock() {
        if (this.copiedValue?.copied) {
            this.dispatchEvent(new PcbValuePastedEvent(this.copiedValue.copied, Section.CONTENT, this.index));
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

                    ${this.copiedValue?.copied
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
