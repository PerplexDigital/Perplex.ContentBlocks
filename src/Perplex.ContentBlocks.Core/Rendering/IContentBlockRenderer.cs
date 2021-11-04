using System.Collections.Generic;
using System;
#if NET5_0
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
#elif NET472
using System.Web;
using System.Web.Mvc;
#endif

namespace Perplex.ContentBlocks.Rendering
{
#if NET5_0
    public delegate Task<IHtmlContent> RenderPartialViewAsync(string partialViewName, object model);
#endif

    public interface IContentBlockRenderer
    {
#if NET5_0

        Task<IHtmlContent> Render(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderHeader(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderBlocks(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderBlock(IContentBlockViewModel contentBlockViewModel, RenderPartialViewAsync renderPartialViewAsync);

        Task<IHtmlContent> RenderBlocks(IEnumerable<IContentBlockViewModel> contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

#elif NET472

        IHtmlString Render(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderHeader(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderBlocks(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderBlock(IContentBlockViewModel contentBlockViewModel, HtmlHelper htmlHelper);

        IHtmlString RenderBlocks(IEnumerable<IContentBlockViewModel> contentBlocks, HtmlHelper htmlHelper);

#endif
    }
}
