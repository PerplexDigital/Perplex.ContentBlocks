using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockSolutions1ViewModel : ContentBlockViewModel<ContentBlockSolutions1>
    {
        public IEnumerable<SolutionCategory> SolutionCategories { get; }

        /// <summary>
        /// Solution category pages by content Id of Solution.
        /// </summary>
        public IDictionary<int, SolutionCategoryPage> SolutionCategoryPages { get; set; }

        public IDictionary<int, string> SolutionPages { get; set; }
        public GlobalText GlobalText { get; }

        public ContentBlockSolutions1ViewModel(
            IGlobalTextAccessor globalTextAccessor,
            IEnumerable<SolutionCategory> solutionCategories, IDictionary<int, SolutionCategoryPage> solutionCategoryPages,
            IDictionary<int, string> solutionPages,
            ContentBlockSolutions1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            SolutionCategories = solutionCategories.ToList();
            SolutionCategoryPages = solutionCategoryPages;
            SolutionPages = solutionPages;
            GlobalText = globalTextAccessor.GlobalText;
        }
    }
}
