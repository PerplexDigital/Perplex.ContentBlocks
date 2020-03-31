using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Usage
{
    public interface IContentBlockUsageService
    {
        IContentBlockUsage GetUsage(Guid contentBlockDefinitionId, int? websiteId, string culture = null);

        IDictionary<Guid, IContentBlockUsage> GetAll(int? websiteId, string culture = null);
    }
}
