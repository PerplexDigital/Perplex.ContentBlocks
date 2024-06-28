using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlockViewModel<TContent>(TContent content, Guid id, Guid definitionId, Guid layoutId) : IContentBlockViewModel<TContent> where TContent : IPublishedElement
{
    public Guid Id { get; } = id;

    public Guid DefinitionId { get; set; } = definitionId;

    public Guid LayoutId { get; set; } = layoutId;

    public TContent Content { get; set; } = content;

    IPublishedElement IContentBlockViewModel.Content => Content;
}
