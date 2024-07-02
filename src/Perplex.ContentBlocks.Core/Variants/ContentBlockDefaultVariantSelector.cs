using Perplex.ContentBlocks.PropertyEditor.Value;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Variants;

/// <summary>
/// Variant selector that never selects a variant but will select the default block content.
/// </summary>
public class ContentBlockDefaultVariantSelector : IContentBlockVariantSelector
{
    public ContentBlockVariantValue? SelectVariant(ContentBlockValue block, IPublishedElement content, bool preview)
        => null;
}
