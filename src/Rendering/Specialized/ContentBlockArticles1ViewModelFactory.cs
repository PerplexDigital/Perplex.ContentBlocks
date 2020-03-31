using Athlon.Features.Articles;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockArticles1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockArticles1>
    {
        private readonly IGlobalTextAccessor _globalTextAccessor;
        private readonly IArticlesAccessor _articlesAccessor;

        public ContentBlockArticles1ViewModelFactory(IGlobalTextAccessor globalTextAccessor, IArticlesAccessor articlesAccessor)
        {
            _globalTextAccessor = globalTextAccessor;
            _articlesAccessor = articlesAccessor;
        }

        public override IContentBlockViewModel<ContentBlockArticles1> Create(ContentBlockArticles1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockArticlesViewModel(_globalTextAccessor, _articlesAccessor, content, id, definitionId, layoutId);
    }
}
