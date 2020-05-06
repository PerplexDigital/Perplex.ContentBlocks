using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Presets
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Upgrade)]
    public class ContentBlockPresetsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IContentBlocksPresetRepository, InMemoryContentBlocksPresetRepository>();
        }
    }
}
