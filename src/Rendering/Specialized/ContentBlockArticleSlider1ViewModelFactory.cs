using Athlon.Features.Articles;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockArticleSlider1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockArticleSlider1>
    {
        private readonly IArticlesAccessor _articlesAccessor;

        public ContentBlockArticleSlider1ViewModelFactory(IArticlesAccessor articleAccessor)
        {
            _articlesAccessor = articleAccessor;
        }

        public override IContentBlockViewModel<ContentBlockArticleSlider1> Create(ContentBlockArticleSlider1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockArticleSlider1ViewModel(_articlesAccessor, content, id, definitionId, layoutId);
    }
}