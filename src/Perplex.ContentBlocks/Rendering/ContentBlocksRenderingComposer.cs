using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Rendering
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksRenderingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Renderer
            composition.Register<IContentBlockRenderer, ContentBlockRenderer>(Lifetime.Singleton);

            // General View Model factory
            composition.Register(
                typeof(IContentBlockViewModelFactory<>),
                typeof(ContentBlockViewModelFactory<>), Lifetime.Singleton);
        }
    }
}
