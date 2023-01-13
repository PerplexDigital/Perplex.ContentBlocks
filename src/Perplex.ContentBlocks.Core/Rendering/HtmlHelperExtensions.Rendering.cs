using System.Collections.Generic;
using System.Linq;

#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
#elif NET472
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Rendering
{
    public static partial class HtmlHelperExtensions
    {
#if NET5_0_OR_GREATER

        /// <summary>
        /// Renders all Content Blocks.
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="contentBlocks">Content Blocks to render</param>
        /// <param name="renderer">Content Blocks renderer</param>
        /// <returns></returns>
        public static async Task<IHtmlContent> RenderContentBlocks(this IHtmlHelper html, IContentBlocks contentBlocks, IContentBlockRenderer renderer)
            => await renderer.Render(contentBlocks, html.PartialAsync);

        /// <summary>
        /// Renders a single Content Block
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="contentBlock">Content Block to render</param>
        /// <param name="renderer">Content Blocks renderer</param>
        /// <returns></returns>
        public static async Task<IHtmlContent> RenderContentBlock(this IHtmlHelper html, IContentBlockViewModel contentBlock, IContentBlockRenderer renderer)
            => await renderer.RenderBlock(contentBlock, html.PartialAsync);

        /// <summary>
        /// Renders multiple Content Blocks
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="contentBlocks">Content Blocks to render</param>
        /// <param name="renderer">Content Blocks renderer</param>
        /// <returns></returns>
        public static async Task<IHtmlContent> RenderContentBlocks(this IHtmlHelper html, IEnumerable<IContentBlockViewModel> contentBlocks, IContentBlockRenderer renderer)
            => await renderer.RenderBlocks(contentBlocks, html.PartialAsync);

#elif NET472
        /// <summary>
        /// Renders all Content Blocks.
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="contentBlocks">Content Blocks to render</param>
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

        /// <summary>
        /// Renders a single Content Block
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="contentBlock">Content Block to render</param>
        /// <returns></returns>
        public static IHtmlString RenderContentBlock(this HtmlHelper html, IContentBlockViewModel contentBlock)
        {
            if (contentBlock == null)
            {
                return MvcHtmlString.Empty;
            }

            var renderer = Current.Factory.GetInstance<IContentBlockRenderer>();
            return renderer.RenderBlock(contentBlock, html);
        }

        /// <summary>
        /// Renders multiple Content Blocks
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="contentBlocks">Content Blocks to render</param>
        /// <returns></returns>
        public static IHtmlString RenderContentBlocks(this HtmlHelper html, IEnumerable<IContentBlockViewModel> contentBlocks)
        {
            if (contentBlocks?.Any() != true)
            {
                return MvcHtmlString.Empty;
            }

            var renderer = Current.Factory.GetInstance<IContentBlockRenderer>();
            return renderer.RenderBlocks(contentBlocks, html);
        }
#endif
    }
}
