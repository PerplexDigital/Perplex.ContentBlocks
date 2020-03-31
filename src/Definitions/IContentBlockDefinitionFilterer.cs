using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions
{
    public interface IContentBlockDefinitionFilterer
    {
        IEnumerable<IContentBlockDefinition> FilterForCulture(IEnumerable<IContentBlockDefinition> definitions, string culture);

        IEnumerable<IContentBlockDefinition> FilterForDocumentType(IEnumerable<IContentBlockDefinition> definitions, string documentType);

        IEnumerable<IContentBlockDefinition> FilterForCultureAndDocumentType(IEnumerable<IContentBlockDefinition> definitions,
            string culture, string documentType);

        IEnumerable<IContentBlockDefinition> FilterForPage(IEnumerable<IContentBlockDefinition> definitions, int pageId, string culture);

        IEnumerable<IContentBlockDefinition> FilterForPage(IEnumerable<IContentBlockDefinition> definitions, string documentType, string culture);
    }
}
