using System;
using Umbraco.Core.Models.PublishedContent;

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
