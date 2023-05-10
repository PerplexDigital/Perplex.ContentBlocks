#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#elif NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Presets
{
#if NET6_0_OR_GREATER
    public class ContentBlockPresetsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IContentBlocksPresetRepository, InMemoryContentBlocksPresetRepository>();
        }
    }
#elif NETFRAMEWORK
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockPresetsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IContentBlocksPresetRepository, InMemoryContentBlocksPresetRepository>();
        }
    }
#endif    
}
