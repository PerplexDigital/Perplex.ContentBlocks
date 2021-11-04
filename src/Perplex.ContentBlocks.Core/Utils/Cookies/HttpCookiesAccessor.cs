using System.Collections.Generic;

#if NET5_0
using Microsoft.AspNetCore.Http;
#elif NET472
using System.Web;
using Umbraco.Web;
#endif

namespace Perplex.ContentBlocks.Utils.Cookies
{
    public class HttpCookiesAccessor : IHttpCookiesAccessor
    {
        public IDictionary<string, string> Cookies { get; }

        public HttpCookiesAccessor(IHttpContextAccessor httpContextAccessor)
        {
            Cookies = new Dictionary<string, string>();

#if NET5_0
            if (httpContextAccessor.HttpContext is HttpContext httpCtx &&
                httpCtx.Request.Cookies is IRequestCookieCollection cookies)
            {
                foreach (var kv in cookies)
                {
                    Cookies[kv.Key] = kv.Value;
                }
            }
#elif NET472
            if (httpContextAccessor.HttpContext is HttpContext httpCtx &&
                httpCtx.Request?.Cookies is HttpCookieCollection cookies)
            {
                foreach (var key in cookies.AllKeys)
                {
                    Cookies[key] = cookies[key]?.Value;
                }
            }
#endif
        }
    }
}
