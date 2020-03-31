using System.Collections.Generic;
using Athlon.Infrastructure.ModelsBuilder;
using Umbraco.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Usage
{
    public interface IContentBlockUsageAnalyzer
    {
        IEnumerable<IContentBlockUsage> GetUsesForPage(IPublishedContent page, string culture);

        IEnumerable<IContentBlockUsage> GetAllUses();
    }
}
