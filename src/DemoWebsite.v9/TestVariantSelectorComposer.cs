using Perplex.ContentBlocks.Variants;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace DemoWebsite.v9
{
    [ComposeAfter(typeof(ContentBlocksVariantsComposer))]
    public class TestVariantSelectorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IContentBlockVariantSelector, TestVariantSelector>();
        }
    }
}
