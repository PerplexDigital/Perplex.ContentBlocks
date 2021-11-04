#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.Utils.Cookies
{
#if NET5_0
public class CookiesComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IHttpCookiesAccessor, HttpCookiesAccessor>();
        }
    }
#elif NET472
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
