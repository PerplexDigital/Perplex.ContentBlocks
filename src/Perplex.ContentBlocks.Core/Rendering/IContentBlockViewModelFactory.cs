using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public interface IContentBlockViewModelFactory
{
    IContentBlockViewModel? Create(IPublishedElement content, Guid id, Guid definitionId, Guid layoutId);
}
