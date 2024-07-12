import {PerplexBlockDefinition} from "../types.ts";

const DEFINITIONS_ENDPOINT = '/umbraco/perplex-content-blocks/api/definitions/all';

export const fetchAllDefinitions = async (token: string) => {
    try {
        const result = await fetch(DEFINITIONS_ENDPOINT, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            }
        });

        return await result.json() as PerplexBlockDefinition[];
    } catch (e) {
        // Todo error handling
        console.log(e)
    }
}
