using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering;

public interface IContentBlockViewModel<out TContent> : IContentBlockViewModel where TContent : IPublishedElement
{
    new TContent Content { get; }
}
