using Perplex.ContentBlocks.Categories;
using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlocksDefinitionApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentBlockDefinitionRepository _definitionRepository;
        private readonly IContentBlockCategoryRepository _categoryRepository;

        public ContentBlocksDefinitionApiController(IContentBlockDefinitionRepository definitionRepository, IContentBlockCategoryRepository categoryRepository)
        {
            _definitionRepository = definitionRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IEnumerable<IContentBlockDefinition> GetAllDefinitions()
            => _definitionRepository.GetAll();

        [HttpGet]
        public IEnumerable<IContentBlockCategory> GetAllCategories()
            => _categoryRepository.GetAll(true);

        [HttpGet]
        public IEnumerable<IContentBlockDefinition> GetDefinitionsForPage(string documentType, string culture)
            => _definitionRepository.GetAllForPage(documentType, culture);
    }
}
