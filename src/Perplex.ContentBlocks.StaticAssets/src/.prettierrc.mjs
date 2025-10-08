export default {
    printWidth: 120,
    singleQuote: true,
    tabWidth: 4,
    singleAttributePerLine: true,
    endOfLine: 'auto',
    overrides: [
        {
            files: ['*.json', '*.yml'],
            options: {
                tabWidth: 2,
            },
        },
    ],
};
