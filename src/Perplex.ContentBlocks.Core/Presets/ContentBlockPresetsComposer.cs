#if NET5_0
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Presets
{
#if NET5_0
    public class ContentBlockPresetsComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IContentBlocksPresetRepository, InMemoryContentBlocksPresetRepository>();
        }
    }
#elif NET472
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
