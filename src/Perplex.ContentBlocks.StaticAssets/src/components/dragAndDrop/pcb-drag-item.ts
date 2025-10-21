// pcb-drag-item.ts
import { LitElement, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { PcbDragAndDrop } from './pcb-drag-and-drop';

@customElement('pcb-drag-item')
export class PcbDragItem extends LitElement {
    @property({ type: Boolean, reflect: true })
    dragging: boolean | null = null;

    @property({
        type: Boolean,
        reflect: true,
        attribute: 'draggable',
        converter: {
            toAttribute: (value: boolean) => (value ? 'true' : 'false'),
            fromAttribute: (value: string | null) => value === 'true',
        },
    })
    draggable: boolean = true;

    @property({ type: String, reflect: true })
    blockId!: string;

    connectedCallback() {
        super.connectedCallback();
        this.addEventListener('dragstart', this.onDragStart);
        this.addEventListener('dragend', this.onDragEnd);
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.removeEventListener('dragstart', this.onDragStart);
        this.removeEventListener('dragend', this.onDragEnd);
    }

    onDragStart = (event: DragEvent) => {
        this.dragging = true;
        const rect = this.getBoundingClientRect();
        PcbDragAndDrop.activeDrag = { element: this, height: rect.height };
        event.dataTransfer!.effectAllowed = 'move';
        event.dataTransfer!.setData('text/plain', '');
    };

    onDragEnd = () => {
        this.dragging = null;
        PcbDragAndDrop.activeDrag = null;
    };

    render() {
        return html`<slot></slot>`;
    }
}
