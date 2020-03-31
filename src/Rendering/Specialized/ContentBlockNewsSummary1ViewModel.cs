using Athlon.Features.News;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockNewsSummary1ViewModel : ContentBlockViewModel<ContentBlockNewsSummary1>
    {
        public IEnumerable<Nieuwsbericht> NewsItems { get; }
        public string LabelReadMore { get; }

        public ContentBlockNewsSummary1ViewModel(IGlobalTextAccessor globalTextAccessor, INieuwsAccessor nieuwsAccessor,
            ContentBlockNewsSummary1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            NewsItems = content.NewsItems.Any() ? content.NewsItems :
                        nieuwsAccessor.GetLatestNews(3);

            LabelReadMore = globalTextAccessor.GlobalText?.LabelReadMore;
        }
    }
}
