using System;

#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NETFRAMEWORK
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Rendering
{
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
}
