export function withEditorId<T>(event: CustomEvent<T>, editorId: string): CustomEvent<T & { editorId: string }> {
    return new CustomEvent<T & { editorId: string }>(event.type, {
        detail: { ...(event.detail as T), editorId },
        bubbles: event.bubbles ?? true,
        composed: event.composed ?? true,
        cancelable: event.cancelable ?? true,
    });
}
