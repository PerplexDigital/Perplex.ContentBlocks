#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.Utils.Cookies
{
#if NET6_0_OR_GREATER
    public class CookiesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IHttpCookiesAccessor, HttpCookiesAccessor>();
        }
    }
#elif NETFRAMEWORK
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class CookiesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IHttpCookiesAccessor, HttpCookiesAccessor>(Lifetime.Scope);
        }
    }
#endif   
}
