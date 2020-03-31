using Athlon.Rendering;
using Athlon.Infrastructure.ModelsBuilder;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Infrastructure.Extensions
{
    public static partial class HtmlHelperExtensions
    {
        /// <summary>
        /// Rendert de contentblocks op de pagina.
        /// De header wordt in een @section Header gerenderd eventueel samen met de breadcrumbs en met een header element eromheen.
        /// De overige blokken worden gewoon los gerenderd.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="contentBlocks"></param>
        /// <returns></returns>
        public static IHtmlString RenderContentBlocks(this HtmlHelper html, IContentBlocksPage contentBlocksPage, INavigatie navigatie, bool renderBreadcrumbs = false)
        {
            if (!(contentBlocksPage?.ContentBlocks is IContentBlocks contentBlocks))
            {
                return MvcHtmlString.Empty;
            }

            var renderer = Current.Factory.GetInstance<IContentBlockRenderer>();

            WebViewPage page = html.ViewDataContainer as WebViewPage;

            if (contentBlocks?.Header != null)
            {
                page.DefineSection("Header", () =>
                {
                    page.Write(MvcHtmlString.Create("<header>"));

                    if (renderBreadcrumbs && navigatie != null)
                    {
                        page.Write(html.Breadcrumbs(contentBlocksPage, navigatie));
                    }

                    page.Write(renderer.RenderHeader(contentBlocks, html));

                    page.Write(MvcHtmlString.Create("</header>"));
                });
            }

            return renderer.RenderBlocks(contentBlocks, html);
        }
    }
}
