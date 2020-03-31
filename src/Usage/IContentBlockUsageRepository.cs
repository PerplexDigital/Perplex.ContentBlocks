using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Usage
{
    public interface IContentBlockUsageRepository
    {
        IContentBlockUsage GetUsage(Guid contentBlockDefinitionId, int? websiteId = null, string culture = null);

        IEnumerable<IContentBlockUsage> GetAllUses(int? websiteId = null, string culture = null);

        void Clear();

        void ClearDefinition(Guid contentBlockDefinitionId, string culture = null);

        void ClearPage(Guid pageId, string culture = null);

        void Clear(Guid contentBlockDefinitionId, Guid pageId, string culture);

        void Save(IContentBlockUsage usage);

        void Save(IEnumerable<IContentBlockUsage> uses);

        void WithLock(Action action);
    }
}
