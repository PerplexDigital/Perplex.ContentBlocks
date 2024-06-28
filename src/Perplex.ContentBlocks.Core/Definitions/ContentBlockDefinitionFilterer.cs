namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinitionFilterer : IContentBlockDefinitionFilterer
{
    public IEnumerable<IContentBlockDefinition> FilterForCulture(IEnumerable<IContentBlockDefinition> definitions, string? culture)
        => definitions.Where(definition =>
            definition.LimitToCultures?.Any() != true ||
            definition.LimitToCultures.Any(definitionCulture => string.Equals(definitionCulture, culture, StringComparison.InvariantCultureIgnoreCase)));

    public IEnumerable<IContentBlockDefinition> FilterForDocumentType(IEnumerable<IContentBlockDefinition> definitions, string documentType)
        => definitions.Where(definition =>
            definition.LimitToDocumentTypes?.Any() != true ||
            definition.LimitToDocumentTypes.Any(definitionDocumentType => string.Equals(definitionDocumentType, documentType, StringComparison.InvariantCultureIgnoreCase)));

    public IEnumerable<IContentBlockDefinition> FilterForCultureAndDocumentType(IEnumerable<IContentBlockDefinition> definitions, string? culture, string documentType)
        => FilterForCulture(FilterForDocumentType(definitions, documentType), culture);

    public IEnumerable<IContentBlockDefinition> FilterForPage(IEnumerable<IContentBlockDefinition> definitions, string? documentType, string? culture)
    {
        if (string.IsNullOrEmpty(documentType))
        {
            return definitions;
        }

        return FilterForCultureAndDocumentType(
            definitions,
            culture,
            documentType);
    }
}
