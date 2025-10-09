import { PCBCategory, PerplexBlockDefinition } from '../types.ts';
import { DefinitionsDictionary } from '../state/slices/definitions.ts';

const DEFINITIONS_ENDPOINT = '/umbraco/perplex-content-blocks/api/definitions/all';
const CATEGORIES_ENDPOINT = '/umbraco/perplex-content-blocks/api/categories/all';

export const fetchAllDefinitions = async (token: string) => {
    try {
        const result = await fetch(DEFINITIONS_ENDPOINT, {
            method: 'GET',
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return (await result.json()) as PerplexBlockDefinition[];
    } catch (e) {
        // Todo error handling
        console.log(e);
    }
};

export const fetchAllCategories = async (token: string) => {
    try {
        const result = await fetch(CATEGORIES_ENDPOINT, {
            method: 'GET',
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });

        return (await result.json()) as PCBCategory[];
    } catch (e) {
        // Todo error handling
        console.log(e);
    }
};

export const fetchDefinitionsPerCategory = async (token: string) => {
    try {
        let definitions = (await fetchAllDefinitions(token)) as PerplexBlockDefinition[];
        const categories = (await fetchAllCategories(token)) as PCBCategory[];

        const definitionsDictionary = definitions.reduce((acc: DefinitionsDictionary, curr: PerplexBlockDefinition) => {
            acc[curr.id] = {
                ...curr,
            };

            return acc;
        }, {});

        // group definitionsDictionary by category
        const groupedDefinitions = categories.map((category) => {
            return {
                category,
                definitions: Object.keys(definitionsDictionary)
                    .filter((key) => definitionsDictionary[key].categoryIds.includes(category.id))
                    .reduce((acc: DefinitionsDictionary, key) => {
                        acc[key] = definitionsDictionary[key];
                        return acc;
                    }, {}),
            };
        });

        return groupedDefinitions;
    } catch (e) {
        // Todo error handling
        console.log(e);
    }
};
