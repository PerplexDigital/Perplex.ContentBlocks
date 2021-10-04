#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Variants
{
#if NET5_0
    public class ContentBlocksVariantsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IContentBlockVariantSelector, ContentBlockDefaultVariantSelector>();
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksVariantsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IContentBlockVariantSelector, ContentBlockDefaultVariantSelector>();
        }
    }
#endif
}
