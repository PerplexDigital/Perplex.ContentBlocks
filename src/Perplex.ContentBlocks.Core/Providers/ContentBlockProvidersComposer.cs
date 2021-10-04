#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.Providers
{
#if NET5_0
    public class ContentBlockProvidersComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IDocumentTypeAliasProvider, DocumentTypeAliasProvider>();
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockProvidersComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IDocumentTypeAliasProvider, DocumentTypeAliasProvider>(Lifetime.Singleton);
        }
    }
#endif 
}
