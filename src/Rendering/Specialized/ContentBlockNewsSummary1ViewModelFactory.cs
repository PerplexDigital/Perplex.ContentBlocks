using Athlon.Features.News;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockNewsSummary1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockNewsSummary1>
    {
        private readonly IGlobalTextAccessor _globalTextAccessor;
        private readonly INieuwsAccessor _nieuwsAccessor;

        public ContentBlockNewsSummary1ViewModelFactory(IGlobalTextAccessor globalTextAccessor, INieuwsAccessor nieuwsAccessor)
        {
            _globalTextAccessor = globalTextAccessor;
            _nieuwsAccessor = nieuwsAccessor;
        }

        public override IContentBlockViewModel<ContentBlockNewsSummary1> Create(ContentBlockNewsSummary1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockNewsSummary1ViewModel(_globalTextAccessor, _nieuwsAccessor, content, id, definitionId, layoutId);
    }
}
