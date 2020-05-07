using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Preview
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Upgrade)]
    public class ContentBlocksPreviewComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IPreviewModeProvider, CookieBasedPreviewModeProvider>(Lifetime.Scope);

            // Can be replaced by clients
            composition.RegisterUnique<IPreviewScrollScriptProvider, DefaultPreviewScrollScriptProvider>();
        }
    }
}
