#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Definitions
{
#if NET5_0
    public class ContentBlockDefinitionComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IContentBlockDefinitionRepository, InMemoryContentBlockDefinitionRepository>();
            builder.Services.AddSingleton<IContentBlockDefinitionFilterer, ContentBlockDefinitionFilterer>();
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockDefinitionComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IContentBlockDefinitionRepository, InMemoryContentBlockDefinitionRepository>();
            composition.Register<IContentBlockDefinitionFilterer, ContentBlockDefinitionFilterer>(Lifetime.Singleton);
        }
    }
#endif
}
