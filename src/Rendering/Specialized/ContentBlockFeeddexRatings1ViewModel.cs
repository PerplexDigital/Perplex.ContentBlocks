using Athlon.Features.FeeddexRatings;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockFeeddexRatings1ViewModel : ContentBlockViewModel<ContentBlockFeeddexRatings1>
    {
        public FeeddexRating FeeddexRating { get; set; }

        public ContentBlockFeeddexRatings1ViewModel(FeeddexRating rating, ContentBlockFeeddexRatings1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            FeeddexRating = rating;
        }
    }
}