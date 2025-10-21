// pcb-drag-and-drop.ts
import { css, LitElement, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { PcbDragItemElement, PerplexContentBlocksBlock } from '../../types.ts';
import { SetBlocksEvent } from '../../events/block.ts';

interface ActiveDrag {
    element: HTMLElement;
    height: number;
}

@customElement('pcb-drag-and-drop')
export class PcbDragAndDrop extends LitElement {
    @property()
    blocks!: PerplexContentBlocksBlock[];

    static activeDrag: ActiveDrag | null = null;

    private placeholder: HTMLElement = document.createElement('div');

    connectedCallback() {
        super.connectedCallback();
        this.addEventListener('dragover', this.onDragOver);
        this.addEventListener('drop', this.onDrop);
        this.placeholder.className = 'placeholder';
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.removeEventListener('dragover', this.onDragOver);
        this.removeEventListener('drop', this.onDrop);
    }

    onDragOver = (event: DragEvent) => {
        event.preventDefault();
        const active = PcbDragAndDrop.activeDrag;
        if (!active) return;

        if (!this.placeholder.parentElement) {
            active.element.style.display = 'none';
            this.placeholder.style.height = `${active.height}px`;
        }

        const children = Array.from(this.children).filter((c) => c !== active.element && c !== this.placeholder);
        let insertBefore: HTMLElement | null = null;

        for (const child of children) {
            const rect = child.getBoundingClientRect();
            if (event.clientY < rect.top + rect.height / 2) {
                insertBefore = child as HTMLElement;
                break;
            }
        }

        if (insertBefore) {
            this.insertBefore(this.placeholder, insertBefore);
        } else {
            this.appendChild(this.placeholder);
        }
    };

    onDrop = (event: DragEvent) => {
        event.preventDefault();
        const active = PcbDragAndDrop.activeDrag;
        if (!active) return;

        this.placeholder.replaceWith(active.element);
        active.element.style.display = '';
        PcbDragAndDrop.activeDrag = null;

        const reorderedDomItems: PcbDragItemElement[] = Array.from(this.children).filter(
            (child): child is PcbDragItemElement => child instanceof HTMLElement && 'blockId' in child,
        );

        const reorderedItems = reorderedDomItems.map(
            (domItem) => this.blocks.find((block) => block.id === domItem.blockId)!,
        );

        this.dispatchEvent(SetBlocksEvent(reorderedItems));
    };

    static styles = css`
        :host {
            display: block;
            padding: 4rem 0;
        }
        .placeholder {
            border: 2px dashed #aaa;
            margin: 4px 0;
            transition: height 0.1s ease;
        }
    `;
    render() {
        return html`<slot></slot>`;
    }
}
