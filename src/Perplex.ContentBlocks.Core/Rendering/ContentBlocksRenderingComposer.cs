using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlocksRenderingComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Renderer
        builder.Services.AddScoped<IContentBlockRenderer, ContentBlockRenderer>();

        // New renderer that supports view components
        builder.Services.AddSingleton<IContentBlocksRenderer, ContentBlocksRenderer>();

        // General View Model factory
        builder.Services.AddSingleton(
            typeof(IContentBlockViewModelFactory<>),
            typeof(ContentBlockViewModelFactory<>));
    }
}
