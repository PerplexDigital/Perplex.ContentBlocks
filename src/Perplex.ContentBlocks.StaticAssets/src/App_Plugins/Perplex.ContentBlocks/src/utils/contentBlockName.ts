import { PerplexContentBlocksBlock } from '../types.ts';

export default (blockTemplateName: string, contentObj: PerplexContentBlocksBlock ):string => {
    const values = contentObj.content.values;

    // Helper to extract plain string from a value (e.g., handle objects like your "text")
    const getValueAsString = (val: any): string => {
        if (val == null) return "";
        if (typeof val === "string") return val;
        if (typeof val === "object" && "markup" in val) return val.markup;
        return String(val);
    };

    return blockTemplateName.replace(/{{(.*?)}}/g, (_, aliases: string) => {
        const aliasList = aliases.split("|").map(a => a.trim());
        for (const alias of aliasList) {
            const match = values.find(v => v.alias === alias);
            if (match && match.value != null && getValueAsString(match.value).trim() !== "") {
                return getValueAsString(match.value);
            }
        }
        return "";
    });
}
