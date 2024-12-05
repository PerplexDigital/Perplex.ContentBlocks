export const ON_ADD_TOAST = 'ON_ADD_TOAST';

type ToastType = "" | "default" | "positive" | "warning" | "danger";

interface ToastContent {
    headline: string,
    message?: string,
}

export const ToastEvent = (type: ToastType, content: ToastContent) => new CustomEvent(ON_ADD_TOAST, {
    detail: {
        type,
        content,
    },
    bubbles: true,
    cancelable: true,
    composed: true,
});