using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockFaq1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockFaq1>
    {
        public override IContentBlockViewModel<ContentBlockFaq1> Create(ContentBlockFaq1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockFaq1ViewModel(content, id, definitionId, layoutId);
    }
}
