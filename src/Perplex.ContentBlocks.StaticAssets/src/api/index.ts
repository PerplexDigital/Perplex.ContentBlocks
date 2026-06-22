import { client } from '@umbraco-cms/backoffice/external/backend-api';

export const BASE_URL = '/umbraco/perplex-content-blocks/api';

export async function get<T>(path: string): Promise<T | null> {
    const response = await client.get({
        baseUrl: BASE_URL,
        url: path,
        security: [{ scheme: 'bearer', type: 'http' }],
    });

    return response.data as T;
}
