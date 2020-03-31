using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockVideo1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockVideo1>
    {
        private readonly IGlobalTextAccessor _globalTextAccessor;
        private readonly ICookiemeldingAccessor _cookiemeldingAccessor;

        public ContentBlockVideo1ViewModelFactory(IGlobalTextAccessor globalTextAccessor, ICookiemeldingAccessor cookiemeldingAccessor)
        {
            _globalTextAccessor = globalTextAccessor;
            _cookiemeldingAccessor = cookiemeldingAccessor;
        }

        public override IContentBlockViewModel<ContentBlockVideo1> Create(ContentBlockVideo1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockVideo1ViewModel(_globalTextAccessor, _cookiemeldingAccessor, content, id, definitionId, layoutId);
    }
}
