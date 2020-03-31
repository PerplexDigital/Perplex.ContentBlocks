using Athlon.Features.FeeddexRatings;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockFeeddexRatings1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockFeeddexRatings1>
    {
        private readonly IFeeddexRatingProvider _feeddexRatingProvider;

        public ContentBlockFeeddexRatings1ViewModelFactory(IFeeddexRatingProvider feeddexRatingProvider)
        {
            _feeddexRatingProvider = feeddexRatingProvider;
        }

        public override IContentBlockViewModel<ContentBlockFeeddexRatings1> Create(ContentBlockFeeddexRatings1 content, Guid id, Guid definitionId, Guid layoutId)
        {
            if(!Enum.TryParse(content.TypeOfFeed.ToString(), true, out RatingsType ratingType))
            {
                ratingType = RatingsType.Essential;
            }
            var rating = _feeddexRatingProvider.GetFeeddexRating(ratingType);
            return new ContentBlockFeeddexRatings1ViewModel(rating, content, id, definitionId, layoutId);
        }
    }
}