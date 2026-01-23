export type ToastType = '' | 'default' | 'positive' | 'warning' | 'danger';

export interface ToastContent {
    headline: string;
    message?: string;
}

export class PcbToastEvent extends Event {
    public static readonly TYPE = 'PcbToastEvent';
    public readonly toastType: ToastType;
    public readonly content: ToastContent;

    public constructor(toastType: ToastType, content: ToastContent) {
        super(PcbToastEvent.TYPE, { bubbles: true, composed: true, cancelable: true });
        this.toastType = toastType;
        this.content = content;
    }
}
