// pcb-drag-and-drop.ts
import { css, LitElement, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { PcbDragItemElement, PerplexContentBlocksBlock } from '../../types.ts';
import { PcbSetBlocksEvent } from '../../events/block.ts';

interface ActiveDrag {
    element: PcbDragItemElement;
    height: number;
}

@customElement('pcb-drag-and-drop')
export class PcbDragAndDrop extends LitElement {
    @property()
    blocks!: PerplexContentBlocksBlock[];

    static activeDrag: ActiveDrag | null = null;

    private placeholder: HTMLElement = document.createElement('div');
    private _rafId: number | null = null;
    private _pendingDragEvent: DragEvent | null = null;

    connectedCallback() {
        super.connectedCallback();
        this.addEventListener('dragover', this.onDragOver);
        this.addEventListener('drop', this.onDrop);
        this.addEventListener('dragend', this.onDragEnd);
        this.placeholder.className = 'placeholder';
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        this.removeEventListener('dragover', this.onDragOver);
        this.removeEventListener('drop', this.onDrop);
        this.removeEventListener('dragend', this.onDragEnd);
        if (this._rafId !== null) {
            cancelAnimationFrame(this._rafId);
            this._rafId = null;
        }
    }

    onDragOver = (event: DragEvent) => {
        event.preventDefault();
        this._pendingDragEvent = event;

        if (this._rafId !== null) return;

        this._rafId = requestAnimationFrame(() => {
            this._rafId = null;
            this._processDragOver();
        });
    };

    private _processDragOver() {
        const event = this._pendingDragEvent;
        if (!event) return;

        const active = PcbDragAndDrop.activeDrag;
        if (!active) return;

        this.placeholder.setAttribute('blockId', active.element.blockId);

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
    }

    onDrop = (event: DragEvent) => {
        event.preventDefault();
        const active = PcbDragAndDrop.activeDrag;
        if (!active) return;

        active.element.style.display = '';
        PcbDragAndDrop.activeDrag = null;

        const reorderedDomItems: string[] = Array.from(this.children)
            .filter((child) => child != active.element && child instanceof HTMLElement && child.hasAttribute('blockId'))
            .map((child) => (child as HTMLElement).getAttribute('blockId')!);

        const reorderedItems = reorderedDomItems.map((domItem) => this.blocks.find((block) => block.id === domItem)!);

        this.dispatchEvent(new PcbSetBlocksEvent(reorderedItems));
    };

    onDragEnd = () => {
        this.resetDrag();
    };

    resetDrag() {
        if (this.placeholder.parentElement) {
            this.placeholder.remove();
            this.placeholder.removeAttribute('blockId');
        }

        PcbDragAndDrop.activeDrag = null;
    }

    static styles = css`
        :host {
            display: block;
            padding: 2rem 0 0 0;
            width: 100%;
            overflow: hidden;
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
