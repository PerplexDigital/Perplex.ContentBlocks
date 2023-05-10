#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Utils
{
#if NET6_0_OR_GREATER
    public class ContentBlockUtilsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ContentBlockUtils>();
        }
    }
#elif NETFRAMEWORK
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class ContentBlockUtilsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ContentBlockUtils>(Lifetime.Singleton);
        }
    }
#endif    
}
