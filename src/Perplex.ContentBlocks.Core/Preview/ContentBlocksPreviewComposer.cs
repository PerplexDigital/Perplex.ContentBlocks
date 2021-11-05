#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.Preview
{
#if NET5_0
    public class ContentBlocksPreviewComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IPreviewModeProvider, CookieBasedPreviewModeProvider>();

            // Can be replaced by clients
            builder.Services.AddUnique<IPreviewScrollScriptProvider, DefaultPreviewScrollScriptProvider>();
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksPreviewComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IPreviewModeProvider, CookieBasedPreviewModeProvider>(Lifetime.Scope);

            // Can be replaced by clients
            composition.RegisterUnique<IPreviewScrollScriptProvider, DefaultPreviewScrollScriptProvider>();
        }
    }
#endif
}
