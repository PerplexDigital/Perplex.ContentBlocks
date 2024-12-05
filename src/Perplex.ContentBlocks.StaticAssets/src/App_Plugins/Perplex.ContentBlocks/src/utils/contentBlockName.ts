export default (template: string, properties: { [key: string]: any }): string => {
    return template.replace(/{{\s*([^}]+)\s*}}/g, (_, expression) => {
        const keys = expression.split('||').map((key: string) => key.trim());
        for (const key of keys) {
            if (typeof properties[key] === 'string' && properties[key].trim() !== '') {
                return properties[key];
            }
        }

        return '';
    });
};
