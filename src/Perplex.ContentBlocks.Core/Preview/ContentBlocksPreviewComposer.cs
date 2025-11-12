using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.Preview;

public class ContentBlocksPreviewComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IPreviewModeProvider, CookieBasedPreviewModeProvider>();

        // Can be replaced by clients
        builder.Services.AddSingleton<IPreviewScrollScriptProvider, DefaultPreviewScrollScriptProvider>();
    }
}
