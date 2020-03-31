using Athlon.Features.Articles;
using Athlon.Features.ArticleSlider;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockArticleSlider1ViewModel : ContentBlockViewModel<ContentBlockArticleSlider1>
    {
        private const int _numberOfArticles = 12;

        public List<ArticleSlide> Slides { get; private set; }

        public ContentBlockArticleSlider1ViewModel(IArticlesAccessor articlesAccessor, ContentBlockArticleSlider1 content, Guid id, Guid definitionId, Guid layoutId) 
            : base(content, id, definitionId, layoutId)
        {
            FillAllSlides(articlesAccessor, content);
        }

        private void FillAllSlides(IArticlesAccessor articlesAccessor, ContentBlockArticleSlider1 content)
        {
            var handpickedArticles = content.HandpickedArticles?.Take(_numberOfArticles).ToList();
            Slides = handpickedArticles.Select(x => {
                var article = new Article(x);
                return new ArticleToArticleSlideConverter().Convert(article);
            }).ToList();

            var slideIds = handpickedArticles.Select(x => x.Id).ToList();

            var automaticallyPickedArticles =
                articlesAccessor.Articles.Children<Article>()
                    .Where(x => !slideIds.Contains(x.Id))
                    .OrderByDescending(a => a.CreateDate)
                    .Take(_numberOfArticles - handpickedArticles.Count);

            Slides.AddRange(automaticallyPickedArticles.Select(x => new ArticleToArticleSlideConverter().Convert(x)));
        }
    }
}