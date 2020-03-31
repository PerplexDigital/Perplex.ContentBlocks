using Perplex.ContentBlocks.Categories;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlocksDefinitionApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentBlockDefinitionService _definitionService;
        private readonly IContentBlockCategoryService _categoryService;

        public ContentBlocksDefinitionApiController(IContentBlockDefinitionService definitionService, IContentBlockCategoryService categoryService)
        {
            _definitionService = definitionService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public IEnumerable<IContentBlockDefinition> GetAllDefinitions()
            => _definitionService.GetAll();

        [HttpGet]
        public IEnumerable<IContentBlockCategory> GetAllCategories()
            => _categoryService.GetAll(true);

        [HttpGet]
        public IEnumerable<IContentBlockDefinition> GetDefinitionsForPage(string documentType, string culture)
            => _definitionService.GetAllForPage(documentType, culture);
    }
}
