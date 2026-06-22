import { UUIToastNotificationElement, UUIToastNotificationLayoutElement } from '@umbraco-cms/backoffice/external/uui';
import { PcbToastEvent } from '../events/toast.ts';

export function addToast(event: PcbToastEvent, host: any) {
    const con = host.renderRoot.querySelector('uui-toast-notification-container');
    const toast = document.createElement('uui-toast-notification') as unknown as UUIToastNotificationElement;
    toast.color = event.toastType;
    toast.style.zIndex = '999';
    const toastLayout = document.createElement(
        'uui-toast-notification-layout',
    ) as unknown as UUIToastNotificationLayoutElement;
    toastLayout.headline = event.content.headline;
    toast.appendChild(toastLayout);

    const messageEl = document.createElement('span');
    messageEl.innerHTML = event.content.message ?? '';
    toastLayout.appendChild(messageEl);

    if (con) {
        con.appendChild(toast);
    }
}
