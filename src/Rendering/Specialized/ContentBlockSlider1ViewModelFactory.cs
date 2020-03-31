using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockSlider1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockSlider1>
    {
        private readonly IGlobalTextAccessor _globalTextAccessor;

        public ContentBlockSlider1ViewModelFactory(IGlobalTextAccessor globalTextAccessor)
        {
            _globalTextAccessor = globalTextAccessor;
        }

        public override IContentBlockViewModel<ContentBlockSlider1> Create(ContentBlockSlider1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockSlider1ViewModel(_globalTextAccessor.GlobalText, content, id, definitionId, layoutId);
    }
}
