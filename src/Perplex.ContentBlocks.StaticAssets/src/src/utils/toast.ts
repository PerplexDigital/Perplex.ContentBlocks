import { UUIToastNotificationElement, UUIToastNotificationLayoutElement } from "@umbraco-cms/backoffice/external/uui";

export function addToast(event: CustomEvent, host: any) {
    const con = host.renderRoot.querySelector('uui-toast-notification-container');
    const toast = document.createElement(
        'uui-toast-notification',
    ) as unknown as UUIToastNotificationElement;
    toast.color = event.detail.type;
    toast.style.zIndex = "999";
    const toastLayout = document.createElement(
        'uui-toast-notification-layout',
    ) as unknown as UUIToastNotificationLayoutElement;
    toastLayout.headline = event.detail.content.headline;
    toast.appendChild(toastLayout);

    const messageEl = document.createElement('span');
    messageEl.innerHTML = event.detail.content.message ?? '';
    toastLayout.appendChild(messageEl);

    if (con) {
        con.appendChild(toast);
    }
}