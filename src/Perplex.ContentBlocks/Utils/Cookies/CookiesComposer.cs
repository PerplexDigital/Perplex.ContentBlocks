using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Utils.Cookies
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Install)]
    public class CookiesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IHttpCookiesAccessor, HttpCookiesAccessor>(Lifetime.Scope);
        }
    }
}
