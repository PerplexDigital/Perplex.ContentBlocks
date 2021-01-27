using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Rendering
{
    public static partial class HtmlHelperExtensions
    {
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
    }
}
