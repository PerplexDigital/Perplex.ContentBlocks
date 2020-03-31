using Athlon.Infrastructure.ModelsBuilder;
using Umbraco.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Extensions
{
    public static class ContentBlocksPageExtensions
    {
        /// <summary>
        /// Levert de Image op uit de Header van deze ContentBlocksPage.
        /// </summary>
        /// <param name="contentBlocksPage"></param>
        /// <returns></returns>
        public static IPublishedContent GetHeaderImage(this IContentBlocksPage contentBlocksPage)
        {
            return contentBlocksPage?.ContentBlocks?.Header?.Content?.GetImage();
        }
    }
}
