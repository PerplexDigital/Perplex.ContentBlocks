using Athlon.Definitions;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockSlider1ViewModel : ContentBlockViewModel<ContentBlockSlider1>
    {
        public GlobalText GlobalText { get; }

        public ContentBlockSlider1ViewModel(GlobalText globalText, ContentBlockSlider1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            GlobalText = globalText;
        }
    }
}
