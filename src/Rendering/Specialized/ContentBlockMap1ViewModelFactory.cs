using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockMap1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockMap1>
    {
        private readonly IGlobaalBeheerAccessor _globaalBeheerAccessor;
        private readonly IBeheerAccessor _beheerAccessor;

        public ContentBlockMap1ViewModelFactory(IGlobaalBeheerAccessor globaalBeheerAccessor, IBeheerAccessor beheerAccessor)
        {
            _globaalBeheerAccessor = globaalBeheerAccessor;
            _beheerAccessor = beheerAccessor;
        }

        public override IContentBlockViewModel<ContentBlockMap1> Create(ContentBlockMap1 content, Guid id, Guid definitionId, Guid layoutId)
        {
            return new ContentBlockMap1ViewModel(_globaalBeheerAccessor, _beheerAccessor, content, id, definitionId, layoutId);
        }
    }
}