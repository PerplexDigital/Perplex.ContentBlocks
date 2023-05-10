using System.Collections.Generic;
using System;
#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
#elif NETFRAMEWORK
using System.Web;
using System.Web.Mvc;
#endif

namespace Perplex.ContentBlocks.Rendering
{
#if NET6_0_OR_GREATER
    public delegate Task<IHtmlContent> RenderPartialViewAsync(string partialViewName, object model);
#endif

    public interface IContentBlockRenderer
    {
#if NET6_0_OR_GREATER

        Task<IHtmlContent> Render(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderHeader(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderBlocks(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderBlock(IContentBlockViewModel contentBlockViewModel, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderBlocks(IEnumerable<IContentBlockViewModel> contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

#elif NETFRAMEWORK

        IHtmlString Render(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderHeader(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderBlocks(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderBlock(IContentBlockViewModel contentBlockViewModel, HtmlHelper htmlHelper);

        IHtmlString RenderBlocks(IEnumerable<IContentBlockViewModel> contentBlocks, HtmlHelper htmlHelper);

#endif
    }
}
