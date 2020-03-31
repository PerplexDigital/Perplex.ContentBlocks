using System.Web;
using System.Web.Mvc;

namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockRenderer
    {
        IHtmlString Render(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderHeader(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderBlocks(IContentBlocks contentBlocks, HtmlHelper htmlHelper);

        IHtmlString RenderBlock(IContentBlockViewModel contentBlockViewModel, HtmlHelper htmlHelper);
    }
}
