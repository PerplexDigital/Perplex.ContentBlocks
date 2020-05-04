using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Preview
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Install)]
    public class ContentBlocksPreviewComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IPreviewModeProvider, CookieBasedPreviewModeProvider>(Lifetime.Scope);
        }
    }
}
