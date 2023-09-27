using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.Variants;

public class ContentBlocksVariantsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddUnique<IContentBlockVariantSelector, ContentBlockDefaultVariantSelector>();
    }
}
