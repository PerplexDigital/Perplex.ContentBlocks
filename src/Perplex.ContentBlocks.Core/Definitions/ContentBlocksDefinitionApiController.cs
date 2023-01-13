using Perplex.ContentBlocks.Categories;
using System.Collections.Generic;


#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mime;
using Umbraco.Cms.Web.BackOffice.Controllers;
#elif NET472
using System.Web.Http;
using Umbraco.Web.WebApi;
#endif

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

#if NET5_0_OR_GREATER
        [HttpGet]
        public IActionResult GetAllDefinitions()
        {
            var definitions = _definitionRepository.GetAll();
            return JsonContent(definitions);
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryRepository.GetAll(true);
            return JsonContent(categories);
        }

        [HttpGet]
        public IActionResult GetDefinitionsForPage(string documentType, string culture)
        {
            var definitions = _definitionRepository.GetAllForPage(documentType, culture);
            return JsonContent(definitions);
        }

        /// <summary>
        /// Returns obj serialized to JSON using Newtonsoft.Json.
        /// The Json serializer + deserializer in .NET Core has been switched 
        /// to the System.Text.Json namespace which handles casing differently.
        /// It will transform properties to camelCase by default instead of using the original casing.
        /// We used PascalCasing in v8 and want to continue to do so to prevent having to rewrite the front-end code,
        /// so we explicitly use Newtonsoft here to create the JSON response by hand.
        /// </summary>
        /// <param name="obj">Object to serialize to JSON</param>
        /// <returns></returns>
        private IActionResult JsonContent(object obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            return Content(serialized, MediaTypeNames.Application.Json);
        }
#elif NET472
        [HttpGet]
        public IEnumerable<IContentBlockDefinition> GetAllDefinitions()
            => _definitionRepository.GetAll();

        [HttpGet]
        public IEnumerable<IContentBlockCategory> GetAllCategories()
            => _categoryRepository.GetAll(true);

        [HttpGet]
        public IEnumerable<IContentBlockDefinition> GetDefinitionsForPage(string documentType, string culture)
            => _definitionRepository.GetAllForPage(documentType, culture);
#endif        
    }
}
