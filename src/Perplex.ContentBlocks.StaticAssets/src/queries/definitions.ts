import { PCBCategory, PerplexBlockDefinition, Preset } from '../types.ts';
import { DefinitionsDictionary } from '../state/slices/definitions.ts';
import { get } from '../api/index.ts';

const DEFINITIONS_ENDPOINT = '/definitions/forpage';
const CATEGORIES_ENDPOINT = '/categories/all';
const PRESETS_ENDPOINT = '/presets/forpage';

export const fetchAllDefinitions = async (documentType: string, culture?: string) => {
    try {
        const params = new URLSearchParams({ documentType });
        if (culture) params.append('culture', culture);
        return await get<PerplexBlockDefinition[]>(`${DEFINITIONS_ENDPOINT}?${params.toString()}`);
    } catch (e) {
        // Todo error handling
        console.log(e);
    }
};

export const fetchAllCategories = async () => {
    try {
        return await get<PCBCategory[]>(CATEGORIES_ENDPOINT);
    } catch (e) {
        // Todo error handling
        console.log(e);
    }
};

export const fetchPagePresets = async (documentType: string, culture?: string) => {
    const params = new URLSearchParams({ documentType });
    if (culture) params.append('culture', culture);

    try {
        return await get<Preset>(`${PRESETS_ENDPOINT}?${params}`);
    } catch (e) {
        console.log(e);
    }
};

export const fetchDefinitionsPerCategory = async (documentType: string, culture?: string) => {
    try {
        const definitions = (await fetchAllDefinitions(documentType, culture)) ?? [];
        const categories = (await fetchAllCategories()) ?? [];

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
