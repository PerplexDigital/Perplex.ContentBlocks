using Athlon.Infrastructure.ModelsBuilder;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockChallenges1ChallengeViewModel
    {
        public Challenge Challenge { get; }
        public IList<Case> RelatedCases { get; }
        public IList<SolutionPage> RelatedSolutionPages { get; }
        public IList<Article> RelatedArticles { get; }

        public ContentBlockChallenges1ChallengeViewModel(
            Challenge challenge,
            IEnumerable<Case> relatedCases,
            IEnumerable<SolutionPage> relatedSolutionPages,
            IEnumerable<Article> relatedArticles)
        {
            Challenge = challenge;
            RelatedCases = relatedCases?.ToList() ?? new List<Case>();
            RelatedSolutionPages = relatedSolutionPages?.ToList() ?? new List<SolutionPage>();
            RelatedArticles = relatedArticles?.ToList() ?? new List<Article>();
        }
    }
}
