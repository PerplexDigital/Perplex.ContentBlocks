import { OpenAPI } from '@umbraco-cms/backoffice/external/backend-api';

export const getToken = async (): Promise<string> => {
    if (typeof OpenAPI.TOKEN === 'string') return OpenAPI.TOKEN;

    if (!OpenAPI.TOKEN) return '';

    // @ts-ignore
    return await OpenAPI.TOKEN({});
};
