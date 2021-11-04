using Perplex.ContentBlocks.Utils.Cookies;

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
            _cookiesAccessor.Cookies.TryGetValue(Constants.Preview.UmbracoPreviewCookieName, out string value) &&
            value == Constants.Preview.UmbracoPreviewCookieValue;
    }
}
