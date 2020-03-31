using Athlon.Features.Solutions;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockSolutions1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockSolutions1>
    {
        private readonly ISolutionsAccessor _solutionsAccessor;
        private readonly IGlobalTextAccessor _globalTextAccessor;

        public ContentBlockSolutions1ViewModelFactory(ISolutionsAccessor solutionsAccessor, IGlobalTextAccessor globalTextAccessor)
        {
            _solutionsAccessor = solutionsAccessor;
            _globalTextAccessor = globalTextAccessor;
        }

        public override IContentBlockViewModel<ContentBlockSolutions1> Create(ContentBlockSolutions1 content, Guid id, Guid definitionId, Guid layoutId)
        {
            var solutionCategories = _solutionsAccessor.GetActiveSolutionCategories();
            var solutionCategoryPages = _solutionsAccessor.GetSolutionCategoryPagesBySolutionCategory();
            var solutionPages = _solutionsAccessor.GetActiveSolutionPagesURLs();

            return new ContentBlockSolutions1ViewModel(
                _globalTextAccessor, solutionCategories, solutionCategoryPages, solutionPages,
                content, id, definitionId, layoutId);
        }
    }
}
