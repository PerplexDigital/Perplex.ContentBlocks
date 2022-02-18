using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System.Linq;

using Umbraco.Cms.Core.Models.PublishedContent;


namespace DemoWebsite.v9
{
    public class TestVariantSelector : Perplex.ContentBlocks.Variants.IContentBlockVariantSelector
    {
        public ContentBlockVariantModelValue SelectVariant(ContentBlockModelValue block, IPublishedElement content, bool preview)
        {
            if (preview)
            {
                // Umbraco / ContentBlocks preview mode -> return default variant
                return null;
            }

            foreach (var variant in block.Variants ?? Enumerable.Empty<ContentBlockVariantModelValue>())
            {
                if (variant.Alias == "test")
                {
                    // If a variant with alias "test" exists for this block we want to render it.
                    // If not then we will just render the default.
                    return variant;
                }
            }

            // Render default
            return null;
        }
    }
}
