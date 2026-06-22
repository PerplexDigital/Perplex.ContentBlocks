using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlockViewModel(IPublishedElement content, Guid definitionId, Guid layoutId) : IContentBlockViewModel
{
    public Guid Id { get; }

    public Guid DefinitionId { get; } = definitionId;

    public Guid LayoutId { get; } = layoutId;

    public IPublishedElement Content { get; } = content;
}
