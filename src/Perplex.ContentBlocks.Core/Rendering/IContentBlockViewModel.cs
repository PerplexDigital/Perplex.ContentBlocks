using System;

#if NET5_0
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Rendering
{
    public interface IContentBlockViewModel
    {
        Guid Id { get; }

        Guid DefinitionId { get; }

        Guid LayoutId { get; }

        IPublishedElement Content { get; }
    }
}
