using Athlon.Features.Articles;
using Athlon.Features.Challenges;
using Athlon.Features.RelatedContent;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockChallenges1ViewModel : ContentBlockViewModel<ContentBlockChallenges1>
    {
        public IEnumerable<ContentBlockChallenges1ChallengeViewModel> ChallengeViewModels { get; }

        public IGlobalTextAccessor GlobalText { get; }

        public ContentBlockChallenges1ViewModel(
            IGlobalTextAccessor globalText,
            IChallengesAccessor challengesAccessor,
            IRelatedCasesAccessor relatedCasesAccessor,
            IRelatedSolutionPagesAccessor relatedSolutionPagesAccessor,
            IArticlesAccessor articlesAccessor, ContentBlockChallenges1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            ChallengeViewModels = CreateChallengeViewModels(challengesAccessor, relatedCasesAccessor, relatedSolutionPagesAccessor, articlesAccessor).ToList();
            GlobalText = globalText;
        }

        private IEnumerable<ContentBlockChallenges1ChallengeViewModel> CreateChallengeViewModels(
            IChallengesAccessor challengesAccessor,
            IRelatedCasesAccessor relatedCasesAccessor,
            IRelatedSolutionPagesAccessor relatedSolutionPagesAccessor,
            IArticlesAccessor articlesAccessor)
        {
            var challenges = challengesAccessor.Challenges?.Children<Challenge>()?.ToList() ?? Enumerable.Empty<Challenge>();

            var relatedCases = relatedCasesAccessor.GetRelatedCases(challenges);
            var relatedSolutionPages = relatedSolutionPagesAccessor.GetRelatedSolutionPages(challenges);
            var relatedArticles = articlesAccessor.GetRelatedArticles(challenges);

            return challenges.Select(challenge =>
            {
                var cases = relatedCases[challenge.Id];
                var solutions = relatedSolutionPages[challenge.Id];
                var articles = relatedArticles[challenge.Id];

                return new ContentBlockChallenges1ChallengeViewModel(challenge, cases, solutions, articles);
            });
        }
    }
}
