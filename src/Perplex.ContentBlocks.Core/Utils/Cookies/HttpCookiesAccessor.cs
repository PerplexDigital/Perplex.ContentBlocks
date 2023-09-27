using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Perplex.ContentBlocks.Utils.Cookies;

public class HttpCookiesAccessor : IHttpCookiesAccessor
{
    public IDictionary<string, string> Cookies { get; }

    public HttpCookiesAccessor(IHttpContextAccessor httpContextAccessor)
    {
        Cookies = new Dictionary<string, string>();

        if (httpContextAccessor.HttpContext is HttpContext httpCtx &&
            httpCtx.Request.Cookies is IRequestCookieCollection cookies)
        {
            foreach (var kv in cookies)
            {
                Cookies[kv.Key] = kv.Value;
            }
        }
    }
}
