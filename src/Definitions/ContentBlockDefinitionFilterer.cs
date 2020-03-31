using Athlon.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlockDefinitionFilterer : IContentBlockDefinitionFilterer
    {
        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;

        public ContentBlockDefinitionFilterer(IDocumentTypeAliasProvider documentTypeAliasProvider)
        {
            _documentTypeAliasProvider = documentTypeAliasProvider;
        }

        public IEnumerable<IContentBlockDefinition> FilterForCulture(IEnumerable<IContentBlockDefinition> definitions, string culture)
            => definitions.Where(definition =>
                definition.LimitToCultures?.Any() != true ||
                definition.LimitToCultures.Any(definitionCulture => string.Equals(definitionCulture, culture, StringComparison.InvariantCultureIgnoreCase)));

        public IEnumerable<IContentBlockDefinition> FilterForDocumentType(IEnumerable<IContentBlockDefinition> definitions, string documentType)
            => definitions.Where(definition =>
                definition.LimitToDocumentTypes?.Any() != true ||
                definition.LimitToDocumentTypes.Any(definitionDocumentType => string.Equals(definitionDocumentType, documentType, StringComparison.InvariantCultureIgnoreCase)));

        public IEnumerable<IContentBlockDefinition> FilterForCultureAndDocumentType(IEnumerable<IContentBlockDefinition> definitions, string culture, string documentType)
            => FilterForCulture(FilterForDocumentType(definitions, documentType), culture);

        public IEnumerable<IContentBlockDefinition> FilterForPage(IEnumerable<IContentBlockDefinition> definitions, int pageId, string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                // Een Document Type kan het vinkje "Vary by Culture" uit hebben staan.
                // In dat geval heeft het blijkbaar helemaal geen culture, dus geven we nu alles terug.
                // Een alternatief is eventueel kijken naar de culture van de bijbehorende Homepage.
                return definitions;
            }

            string documentType = _documentTypeAliasProvider.GetDocumentTypeAlias(pageId);
            return FilterForPage(definitions, documentType, culture);
        }

        public IEnumerable<IContentBlockDefinition> FilterForPage(IEnumerable<IContentBlockDefinition> definitions, string documentType, string culture)
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
}
