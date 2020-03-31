using Umbraco.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockViewModel<TContent> : IContentBlockViewModel where TContent : IPublishedElement
    {
        new TContent Content { get; }
    }
}
