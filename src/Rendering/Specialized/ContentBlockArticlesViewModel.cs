using Athlon.Features.Articles;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockArticlesViewModel : ContentBlockViewModel<ContentBlockArticles1>
    {
        public ArticlesPartialViewModel viewModel { get; }

        public ContentBlockArticlesViewModel(
            IGlobalTextAccessor globalTextAccessor,
            IArticlesAccessor articlesAccessor,
            ContentBlockArticles1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {

            viewModel = new ArticlesPartialViewModel(globalTextAccessor, articlesAccessor, 1, content.NumberOfRows, challenge: null);
        }
    }
}
