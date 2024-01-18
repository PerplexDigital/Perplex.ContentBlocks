using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public interface IContentBlockViewModelFactory<TContent> : IContentBlockViewModelFactory where TContent : IPublishedElement
{
    IContentBlockViewModel<TContent>? Create(TContent content, Guid id, Guid definitionId, Guid layoutId);
}
