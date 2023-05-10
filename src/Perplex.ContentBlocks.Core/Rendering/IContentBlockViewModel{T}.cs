#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NETFRAMEWORK
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockViewModel<TContent> : IContentBlockViewModel where TContent : IPublishedElement
    {
        new TContent Content { get; }
    }
}
