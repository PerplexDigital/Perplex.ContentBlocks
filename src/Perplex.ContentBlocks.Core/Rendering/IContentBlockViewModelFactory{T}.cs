using System;
#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NETFRAMEWORK
using Umbraco.Core.Models.PublishedContent;
#endif


namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockViewModelFactory<TContent> : IContentBlockViewModelFactory where TContent : IPublishedElement
    {
        IContentBlockViewModel<TContent> Create(TContent content, Guid id, Guid definitionId, Guid layoutId);
    }
}
