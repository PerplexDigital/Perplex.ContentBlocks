using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.Presets;

public class ContentBlockPresetsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddUnique<IContentBlocksPresetRepository, InMemoryContentBlocksPresetRepository>();
    }
}
