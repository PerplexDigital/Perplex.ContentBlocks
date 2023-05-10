#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#elif NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Categories
{
#if NET6_0_OR_GREATER
    public class ContentBlockCategoriesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IContentBlockCategoryRepository, InMemoryContentBlockCategoryRepository>();
        }
    }
#elif NETFRAMEWORK
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockCategoriesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IContentBlockCategoryRepository, InMemoryContentBlockCategoryRepository>();
        }
    }
#endif
}
