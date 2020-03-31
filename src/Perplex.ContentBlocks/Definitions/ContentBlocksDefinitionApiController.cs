using Perplex.ContentBlocks.Categories;
using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlocksDefinitionApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentBlockDefinitionRepository _definitionService;
        private readonly IContentBlockCategoryRepository _categoryService;

        public ContentBlocksDefinitionApiController(IContentBlockDefinitionRepository definitionService, IContentBlockCategoryRepository categoryService)
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
