using System;

using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlockViewModel : IContentBlockViewModel
{
    public Guid Id { get; }

    public Guid DefinitionId { get; }

    public Guid LayoutId { get; }

    public IPublishedElement Content { get; }

    public ContentBlockViewModel(IPublishedElement content, Guid definitionId, Guid layoutId)
    {
        Content = content;
        DefinitionId = definitionId;
        LayoutId = layoutId;
    }
}
