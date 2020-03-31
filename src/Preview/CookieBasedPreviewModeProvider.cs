using Athlon.Infrastructure.Accessors;

namespace Perplex.ContentBlocks.Preview
{
    public class CookieBasedPreviewModeProvider : IPreviewModeProvider
    {
        private readonly IHttpCookiesAccessor _cookiesAccessor;

        public CookieBasedPreviewModeProvider(IHttpCookiesAccessor cookiesAccessor)
        {
            _cookiesAccessor = cookiesAccessor;
        }

        public bool IsPreviewMode =>
            _cookiesAccessor.Cookies.TryGetValue(Constants.ContentBlocks.Preview.UmbracoPreviewCookieName, out string value) &&
            value == Constants.ContentBlocks.Preview.UmbracoPreviewCookieValue;
    }
}
