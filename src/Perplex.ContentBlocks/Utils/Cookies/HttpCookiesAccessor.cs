using System.Collections.Generic;
using System.Web;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Utils.Cookies
{
    public class HttpCookiesAccessor : IHttpCookiesAccessor
    {
        public IDictionary<string, string> Cookies { get; }

        public HttpCookiesAccessor(IHttpContextAccessor httpContextAccessor)
        {
            Cookies = new Dictionary<string, string>();

            if (httpContextAccessor.HttpContext is HttpContext httpCtx && httpCtx.Request?.Cookies is HttpCookieCollection cookies)
            {
                foreach (var key in cookies.AllKeys)
                {
                    Cookies[key] = cookies[key]?.Value;
                }
            }
        }
    }
}
