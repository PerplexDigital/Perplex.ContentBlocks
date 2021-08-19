using System;
#if NET5_0
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif


namespace Perplex.ContentBlocks.Rendering
{
    public class ContentBlockViewModelFactory<TContent> : IContentBlockViewModelFactory<TContent> where TContent : class, IPublishedElement
    {
        public virtual IContentBlockViewModel<TContent> Create(TContent content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockViewModel<TContent>(content, id, definitionId, layoutId);

        IContentBlockViewModel IContentBlockViewModelFactory.Create(IPublishedElement content, Guid id, Guid definitionId, Guid layoutId)
            => Create(content as TContent, id, definitionId, layoutId);
    }
}
