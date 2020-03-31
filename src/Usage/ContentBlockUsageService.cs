using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlockUsageService : IContentBlockUsageService
    {
        private readonly IContentBlockUsageRepository _repository;

        public ContentBlockUsageService(IContentBlockUsageRepository repository)
        {
            _repository = repository;
        }

        public IDictionary<Guid, IContentBlockUsage> GetAll(int? websiteId = null, string culture = null)
            => _repository.GetAllUses(websiteId, culture).ToDictionary(u => u.ContentBlockDefinitionId);

        public IContentBlockUsage GetUsage(Guid contentBlockDefinitionId, int? websiteId = null, string culture = null)
            => _repository.GetUsage(contentBlockDefinitionId, websiteId, culture);
    }
}
