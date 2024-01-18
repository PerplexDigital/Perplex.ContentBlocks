using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlocksRenderingComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Renderer
#pragma warning disable CS0618 // For backwards compatibility
        builder.Services.AddScoped<IContentBlockRenderer, ContentBlockRenderer>();
#pragma warning restore CS0618 // For backwards compatibility

        // New renderer that supports view components
        builder.Services.AddSingleton<IContentBlocksRenderer, ContentBlocksRenderer>();

        // General View Model factory
        builder.Services.AddSingleton(
            typeof(IContentBlockViewModelFactory<>),
            typeof(ContentBlockViewModelFactory<>));
    }
}
