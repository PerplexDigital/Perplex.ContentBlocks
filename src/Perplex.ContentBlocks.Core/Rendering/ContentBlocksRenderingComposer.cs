using Microsoft.Extensions.DependencyInjection;

#if NET5_0_OR_GREATER
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Rendering
{
#if NET5_0_OR_GREATER
    public class ContentBlocksRenderingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Renderer
            builder.Services.AddScoped<IContentBlockRenderer, ContentBlockRenderer>();

            // General View Model factory
            builder.Services.AddSingleton(
                typeof(IContentBlockViewModelFactory<>),
                typeof(ContentBlockViewModelFactory<>));
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksRenderingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Renderer
            composition.Register<IContentBlockRenderer, ContentBlockRenderer>(Lifetime.Scope);

            // General View Model factory
            composition.Register(
                typeof(IContentBlockViewModelFactory<>),
                typeof(ContentBlockViewModelFactory<>), Lifetime.Singleton);
        }
    }
#endif
}
