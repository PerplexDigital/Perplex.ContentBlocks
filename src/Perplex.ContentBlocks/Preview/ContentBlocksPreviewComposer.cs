using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Preview
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksPreviewComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IPreviewModeProvider, CookieBasedPreviewModeProvider>(Lifetime.Scope);
        }
    }
}
