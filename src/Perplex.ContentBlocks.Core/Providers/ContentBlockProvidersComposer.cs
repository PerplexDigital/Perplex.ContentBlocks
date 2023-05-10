#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.Providers
{
#if NET6_0_OR_GREATER
    public class ContentBlockProvidersComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IDocumentTypeAliasProvider, DocumentTypeAliasProvider>();
        }
    }
#elif NETFRAMEWORK
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
