using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlockViewModelFactory<TContent> : IContentBlockViewModelFactory<TContent> where TContent : class, IPublishedElement
{
    public virtual IContentBlockViewModel<TContent> Create(TContent content, Guid id, Guid definitionId, Guid layoutId)
        => new ContentBlockViewModel<TContent>(content, id, definitionId, layoutId);

    IContentBlockViewModel? IContentBlockViewModelFactory.Create(IPublishedElement content, Guid id, Guid definitionId, Guid layoutId)
    {
        if (content is not TContent typedContent) return null;
        return Create(typedContent, id, definitionId, layoutId);
    }
}
