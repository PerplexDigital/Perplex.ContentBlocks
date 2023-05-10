using System;

#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NETFRAMEWORK
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockViewModelFactory
    {
        IContentBlockViewModel Create(IPublishedElement content, Guid id, Guid definitionId, Guid layoutId);
    }
}
