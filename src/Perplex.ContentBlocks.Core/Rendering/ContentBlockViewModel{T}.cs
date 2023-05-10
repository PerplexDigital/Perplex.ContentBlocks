using System;

#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NETFRAMEWORK
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Perplex.ContentBlocks.Rendering
{
    public class ContentBlockViewModel<TContent> : IContentBlockViewModel<TContent> where TContent : IPublishedElement
    {
        public Guid Id { get; }

        public Guid DefinitionId { get; set; }

        public Guid LayoutId { get; set; }

        public TContent Content { get; set; }

        IPublishedElement IContentBlockViewModel.Content => Content;

        public ContentBlockViewModel(TContent content, Guid id, Guid definitionId, Guid layoutId)
        {
            Id = id;
            DefinitionId = definitionId;
            LayoutId = layoutId;
            Content = content;
        }
    }
}
