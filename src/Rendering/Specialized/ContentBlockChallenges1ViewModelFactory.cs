using Athlon.Features.Articles;
using Athlon.Features.Challenges;
using Athlon.Features.RelatedContent;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockChallenges1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockChallenges1>
    {
        private readonly IGlobalTextAccessor _globalTextAccessor;
        private readonly IChallengesAccessor _challengesAccessor;
        private readonly IRelatedCasesAccessor _relatedCasesAccessor;
        private readonly IRelatedSolutionPagesAccessor _relatedSolutionPagesAccessor;
        private readonly IArticlesAccessor _relatedArticleAccessor;

        public ContentBlockChallenges1ViewModelFactory(
            IGlobalTextAccessor globalTextAccessor,
            IChallengesAccessor challengesAccessor,
            IRelatedCasesAccessor relatedCasesAccessor,
            IRelatedSolutionPagesAccessor relatedSolutionPagesAccessor,
            IArticlesAccessor relatedArticleAccessor)
        {
            _globalTextAccessor = globalTextAccessor;
            _challengesAccessor = challengesAccessor;
            _relatedCasesAccessor = relatedCasesAccessor;
            _relatedSolutionPagesAccessor = relatedSolutionPagesAccessor;
            _relatedArticleAccessor = relatedArticleAccessor;
        }

        public override IContentBlockViewModel<ContentBlockChallenges1> Create(ContentBlockChallenges1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockChallenges1ViewModel(_globalTextAccessor, _challengesAccessor, _relatedCasesAccessor, _relatedSolutionPagesAccessor, _relatedArticleAccessor, content, id, definitionId, layoutId);
    }
}
