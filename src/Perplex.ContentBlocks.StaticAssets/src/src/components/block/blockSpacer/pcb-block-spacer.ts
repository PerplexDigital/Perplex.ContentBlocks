import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { customElement, html, property, unsafeCSS } from '@umbraco-cms/backoffice/external/lit';

import blockSpacerStyles from './pcb-block-spacer.css?inline';
import { connect } from 'pwa-helpers';
import { store } from '../../../state/store.ts';
import { setAddBlockModal } from '../../../state/slices/ui.ts';
import { Section } from '../../../types.ts';

@customElement('pcb-block-spacer')
export default class PerplexContentBlocksBlockSpacerElement extends connect(store)(UmbLitElement) {
    @property({ type: Number, attribute: 'index' })
    index: number = 0;

    addBlock() {
        store.dispatch(
            setAddBlockModal({
                display: true,
                section: Section.CONTENT,
                insertAtIndex: this.index,
            }),
        );
    }
    render() {
        return html`<div class="pcb-block-spacer">
            <div>
                <div class="pcb-block-spacer__controls">
                    <uui-button
                        look="primary"
                        @click=${this.addBlock}
                    >
                        <slot name="label"> Add content </slot>

                        <slot name="extra">
                            <uui-icon name="icon-add"></uui-icon>
                        </slot>
                    </uui-button>
                </div>
            </div>
        </div>`;
    }

    static styles = [unsafeCSS(blockSpacerStyles)];
}
