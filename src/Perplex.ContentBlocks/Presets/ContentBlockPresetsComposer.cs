using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Presets
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockPresetsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IContentBlocksPresetRepository, InMemoryContentBlocksPresetRepository>(Lifetime.Singleton);
        }
    }
}
