using Athlon.Definitions;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockHeaderLeaseDeals1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockHeaderLeaseDeals1>
    {
        private readonly IContentBlockDefinitionService _contentBlockDefinitionService;
        private readonly ICurrentPublishedContentAccessor _currentPublishedContentAccessor;

        public ContentBlockHeaderLeaseDeals1ViewModelFactory(
            IContentBlockDefinitionService contentBlockDefinitionService, 
            ICurrentPublishedContentAccessor currentPublishedContentAccessor)
        {
            _contentBlockDefinitionService = contentBlockDefinitionService;
            _currentPublishedContentAccessor = currentPublishedContentAccessor;
        }

        public override IContentBlockViewModel<ContentBlockHeaderLeaseDeals1> Create(ContentBlockHeaderLeaseDeals1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockHeaderLeaseDeals1ViewModel(_contentBlockDefinitionService, _currentPublishedContentAccessor, content, id, definitionId, layoutId);
    }
}