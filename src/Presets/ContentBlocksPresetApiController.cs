using Athlon.Categories;
using Athlon.Presets;
using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlocksPresetApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentBlocksPresetService _presetService;

        public ContentBlocksPresetApiController(IContentBlocksPresetService presetService)
        {
            _presetService = presetService;
        }

        [HttpGet]
        public IEnumerable<IContentBlocksPreset> GetAllPresets()
            => _presetService.GetAll();

        [HttpGet]
        public IContentBlocksPreset GetPresetForPage(string documentType, string culture)
            => _presetService.GetPresetForPage(documentType, culture);
    }
}
