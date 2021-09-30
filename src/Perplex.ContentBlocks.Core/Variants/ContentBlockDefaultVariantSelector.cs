using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System.Linq;

#if NET472
using Umbraco.Core.Models.PublishedContent;
#elif NET5_0
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Variants
{
    /// <summary>
    /// Variant selector that never selects a variant but will select the default block content.
    /// </summary>
    public class ContentBlockDefaultVariantSelector : IContentBlockVariantSelector
    {
        public ContentBlockVariantModelValue SelectVariant(ContentBlockModelValue block, IPublishedElement content, bool preview)
            => null;
    }
}
