using Microsoft.AspNetCore.Http;

namespace Perplex.ContentBlocks.Preview;

public class CookieBasedPreviewModeProvider(IHttpContextAccessor httpCtxAcc) : IPreviewModeProvider
{
    public bool IsPreviewMode
        => httpCtxAcc.HttpContext?.Request?.Cookies?.ContainsKey(Umbraco.Cms.Core.Constants.Web.PreviewCookieName) == true;
}
