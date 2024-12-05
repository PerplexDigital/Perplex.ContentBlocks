import { html, customElement, property, nothing, css } from '@umbraco-cms/backoffice/external/lit';
import { PerplexBlockDefinition } from '../types.ts';
import { UUICardElement } from '@umbraco-cms/backoffice/external/uui';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { LitElement } from 'lit';
import { ON_BLOCK_SELECTED } from '../events/block.ts';

@customElement('pcb-block-definition')
export class PcbBlockDefinition extends LitElement {
    @property({ attribute: false })
    definition?: PerplexBlockDefinition;

    @property({ attribute: false })
    disabled?: boolean;

    @property({ attribute: false })
    selected?: boolean;

    @property({ type: String, attribute: 'background' })
    background?: string;

    connectedCallback() {
        super.connectedCallback();
    }

    onSelected() {
        if (!this.definition) return;

        //Todo dynamically select layout
        this.dispatchEvent(
            new CustomEvent(ON_BLOCK_SELECTED, {
                bubbles: true,
                detail: {
                    id: UmbId.new(),
                    definitionId: this.definition.id,
                    layoutId: this.definition?.layouts[0].id,
                    isDisabled: false,
                    content: {
                        key: UmbId.new(),
                        contentTypeKey: this.definition.elementTypeKey,
                        values: [],
                    },
                },
                composed: true,
            }),
        );
    }

    render() {
        if (!this.definition) return html``;
        return html`
            <button
                class="blockDefinition"
                @click=${this.onSelected}
            >
                <div id="portrait">
                    <img
                        src=${this.definition.previewImage}
                        alt="Content block preview image"
                    />
                </div>
                ${this.#renderButton()}

                <div id="tag"></div>
                <div id="actions"></div>
            </button>
        `;
    }

    #renderButton() {
        if (!this.definition) return html``;
        return html`
            <button
                id="open-part"
                tabindex=${this.disabled ? (nothing as any) : '0'}
            >
                <strong>${this.definition.name}</strong>
                <span>${this.definition.description}</span>
            </button>
        `;
    }
    temp = [
        ...UUICardElement.styles,
        css`
            :host {
                flex-direction: column;
                justify-content: flex-start;
                background: white;
            }

            :host([selected='true']) {
                outline: 3px solid var(--uui-color-focus, #3879ff);
                outline-offset: 0;
            }

            :host(:hover) #info {
                color: var(--uui-color-interactive-emphasis);
            }

            .blockDefinition {
                background: none;
                color: inherit;
                border: none;
                padding: 0;
                font: inherit;
                cursor: pointer;
            }

            #portrait {
                background-color: var(--uui-color-surface-alt);
                display: flex;
                justify-content: center;
                min-height: 150px;
                max-height: 150px;

                img {
                    align-self: center;
                    display: block;
                    font-size: var(--uui-size-8);
                    border-radius: var(--uui-border-radius);
                    object-fit: cover;
                    width: 100%;
                    height: 100%;
                }
            }

            #open-part {
                text-align: left;
                background-color: var(--uui-color-surface);
                cursor: pointer;
                color: var(--uui-color-interactive);
                border: none;
                border-top: 1px solid var(--uui-color-divider);
                border-radius: 0 0 var(--uui-border-radius) var(--uui-border-radius);
                font-family: inherit;
                font-size: var(--uui-type-small-size);
                box-sizing: border-box;
                padding: var(--uui-size-2) var(--uui-size-4);
                display: flex;
                flex-direction: column;
                line-height: var(--uui-size-6);

                strong {
                    font-size: var(--uui-size-5);
                }

                span {
                    font-size: var(--uui-size-4);
                }
            }

            :host([disabled]) #open-part {
                pointer-events: none;
                background: var(--uui-color-disabled);
                color: var(--uui-color-contrast-disabled);
            }

            #open-part:hover strong {
                text-decoration: underline;
            }

            #open-part:hover {
                color: var(--uui-color-interactive-emphasis);
            }

            :host([image]:not([image=''])) #open-part {
                transition: opacity 0.5s 0.5s;
                opacity: 0;
            }

            #tag {
                position: absolute;
                top: var(--uui-size-4);
                right: var(--uui-size-4);
                display: flex;
                justify-content: right;
            }

            #actions {
                position: absolute;
                top: var(--uui-size-4);
                right: var(--uui-size-4);
                display: flex;
                justify-content: right;

                opacity: 0;
                transition: opacity 120ms;
            }

            :host(:focus) #actions,
            :host(:focus-within) #actions,
            :host(:hover) #actions {
                opacity: 1;
            }

            :host(
                    [image]:not([image='']):hover,
                    [image]:not([image='']):focus,
                    [image]:not([image='']):focus-within,
                    [selected][image]:not([image='']),
                    [error][image]:not([image=''])
                )
                #open-part {
                opacity: 1;
                transition-duration: 120ms;
                transition-delay: 0s;
            }
        `,
    ];

    static styles = [
        ...UUICardElement.styles,
        css`
            :host {
                flex-direction: column;
                justify-content: flex-start;
                background: white;
            }

            :host([selected='true']) {
                outline: 3px solid var(--uui-color-focus, #3879ff);
                outline-offset: 0;
            }

            :host(:hover) #info {
                color: var(--uui-color-interactive-emphasis);
            }

            .blockDefinition {
                background: none;
                color: inherit;
                border: none;
                padding: 0;
                font: inherit;
                cursor: pointer;
            }

            #portrait {
                background-color: var(--uui-color-surface-alt);
                display: flex;
                justify-content: center;
                height: 150px;

                img {
                    align-self: center;
                    display: block;
                    font-size: var(--uui-size-8);
                    border-radius: var(--uui-border-radius);
                    object-fit: cover;
                    width: 100%;
                    height: 100%;
                }
            }

            #open-part {
                text-align: left;
                background-color: var(--uui-color-surface);
                cursor: pointer;
                color: var(--uui-color-interactive);
                border: none;
                border-top: 1px solid var(--uui-color-divider);
                border-radius: 0 0 var(--uui-border-radius) var(--uui-border-radius);
                font-family: inherit;
                font-size: var(--uui-type-small-size);
                box-sizing: border-box;
                padding: var(--uui-size-2) var(--uui-size-4);
                display: flex;
                flex-direction: column;
                line-height: var(--uui-size-6);

                strong {
                    font-size: var(--uui-size-5);
                }

                span {
                    font-size: var(--uui-size-4);
                }
            }

            :host([disabled]) #open-part {
                pointer-events: none;
                background: var(--uui-color-disabled);
                color: var(--uui-color-contrast-disabled);
            }

            #open-part:hover strong {
                text-decoration: underline;
            }

            #open-part:hover {
                color: var(--uui-color-interactive-emphasis);
            }

            :host([image]:not([image=''])) #open-part {
                transition: opacity 0.5s 0.5s;
                opacity: 0;
            }

            #tag {
                position: absolute;
                top: var(--uui-size-4);
                right: var(--uui-size-4);
                display: flex;
                justify-content: right;
            }

            #actions {
                position: absolute;
                top: var(--uui-size-4);
                right: var(--uui-size-4);
                display: flex;
                justify-content: right;

                opacity: 0;
                transition: opacity 120ms;
            }

            :host(:focus) #actions,
            :host(:focus-within) #actions,
            :host(:hover) #actions {
                opacity: 1;
            }

            :host(
                    [image]:not([image='']):hover,
                    [image]:not([image='']):focus,
                    [image]:not([image='']):focus-within,
                    [selected][image]:not([image='']),
                    [error][image]:not([image=''])
                )
                #open-part {
                opacity: 1;
                transition-duration: 120ms;
                transition-delay: 0s;
            }
        `,
    ];
}

/*export default class PcbBlockDefinition extends UUICardElement {

    @property()
    definition?: PerplexBlockDefinition;



    render() {
        if (!this.definition) return html``;
        return html`
            <div>
                <h1>${this.definition.name}</h1>
                <img
                    src=${this.definition.previewImage}
                    alt=""
                >

                ${this.#renderButton()}

                <uui-tag slot="tag" size="s" color="danger" look="primary">Trashed</uui-tag>

            </div>
        `;
    }

    #renderButton() {
        if (!this.definition) return nothing;
        return html`
      <button
        id="open-part"
        tabindex=${this.disabled ? (nothing as any) : '0'}
        @click=${this.handleOpenClick}
        @keydown=${this.handleOpenKeydown}>
        <strong>${this.definition.name}</strong><small>${this.definition.description}</small>
      </button>
    `;
    }

    static styles = [
        ...UUICardElement.styles,
        css`
            slot[name='tag'] {
                position: absolute;
                top: var(--uui-size-4);
                right: var(--uui-size-4);
                display: flex;
                justify-content: right;
            }
        `
    ];
}*/
