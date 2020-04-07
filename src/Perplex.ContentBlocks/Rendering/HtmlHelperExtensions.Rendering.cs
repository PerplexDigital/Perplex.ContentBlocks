using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Rendering
{
    public static partial class HtmlHelperExtensions
    {
        /// <summary>
        /// Renders the content blocks.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="contentBlocks"></param>
        /// <returns></returns>
        public static IHtmlString RenderContentBlocks(this HtmlHelper html, IContentBlocks contentBlocks)
        {
            if (contentBlocks == null)
            {
                return MvcHtmlString.Empty;
            }

            var renderer = Current.Factory.GetInstance<IContentBlockRenderer>();
            return renderer.Render(contentBlocks, html);
        }
    }
}
