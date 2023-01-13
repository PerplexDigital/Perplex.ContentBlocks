using System;

#if NET5_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockViewModelFactory
    {
        IContentBlockViewModel Create(IPublishedElement content, Guid id, Guid definitionId, Guid layoutId);
    }
}
