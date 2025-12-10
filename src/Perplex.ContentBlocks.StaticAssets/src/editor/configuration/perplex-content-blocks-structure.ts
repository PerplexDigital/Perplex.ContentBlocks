import { html, customElement, property, css } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

@customElement('perplex-content-blocks-structure')
export default class PerplexContentBlocksStructurePropertyEditorUiElement
    extends UmbLitElement
    implements UmbPropertyEditorUiElement
{
    @property({ type: String })
    public value = '';

    private _setValue(value: string) {
        this.value = value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    override render() {
        // Note: we have seen the value come back as an int like 3 but also a string like 'All' from the database.
        // This is probably related to it being a Flags enum.
        // It is no big deal, we simply will check for both but the value we send to the server is the string value.
        return html`
            <select
                id="native"
                @change=${(e: Event) => this._setValue((e.target as HTMLSelectElement).value)}
            >
                <option
                    value="All"
                    ?selected=${this.value === 'All' || this.value == '3'}
                >
                    Header + Blocks
                </option>
                <option
                    value="Header"
                    ?selected=${this.value === 'Header' || this.value == '2'}
                >
                    Header
                </option>
                <option
                    value="Blocks"
                    ?selected=${this.value === 'Blocks' || this.value == '1'}
                >
                    Blocks
                </option>
            </select>
        `;
    }

    // Styles copied from:
    // https://github.com/umbraco/Umbraco.UI/blob/0d855e6203e9ea01c3b231ef55c37fcbddaf24c8/packages/uui-select/lib/uui-select.element.ts#L272
    static styles = css`
        :host {
            display: inline-block;
            position: relative;
            font-family: inherit;
        }

        #native {
            display: inline-block;
            font-family: inherit;
            font-size: var(--uui-select-font-size, inherit);
            height: var(--uui-select-height, var(--uui-size-11));
            padding: var(--uui-select-padding-y, var(--uui-size-1)) var(--uui-select-padding-x, var(--uui-size-2));
            color: var(--uui-select-text-color, var(--uui-color-text));
            box-sizing: border-box;
            border-radius: var(--uui-border-radius);
            border: 1px solid var(--uui-select-border-color, var(--uui-color-border));
            transition: all 150ms ease;
            width: 100%;
            background-color: var(--uui-select-background-color, var(--uui-color-surface));
        }

        #native:focus {
            outline: calc(2px * var(--uui-show-focus-outline, 1)) solid
                var(--uui-select-outline-color, var(--uui-color-focus));
        }

        #native[disabled] {
            cursor: not-allowed;
            background-color: var(--uui-select-disabled-background-color, var(--uui-color-disabled));
        }

        #native:hover {
            border: 1px solid var(--uui-select-border-color-hover, var(--uui-color-border-emphasis));
        }

        option:checked {
            background: var(--uui-select-selected-option-background-color, var(--uui-color-selected));
            color: var(--uui-select-selected-option-color, var(--uui-color-selected-contrast));
        }

        #caret {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
        }

        :host([error]) #native {
            border: 1px solid var(--uui-color-invalid-standalone);
        }

        :host([error]) #native[disabled] {
            border: 1px solid var(--uui-color-invalid-standalone);
        }
    `;
}
