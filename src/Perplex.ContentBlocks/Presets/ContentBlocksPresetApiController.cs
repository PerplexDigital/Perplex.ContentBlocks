using Perplex.ContentBlocks.Presets;
using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlocksPresetApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentBlocksPresetRepository _presetRepository;

        public ContentBlocksPresetApiController(IContentBlocksPresetRepository presetRepository)
        {
            _presetRepository = presetRepository;
        }

        [HttpGet]
        public IEnumerable<IContentBlocksPreset> GetAllPresets()
            => _presetRepository.GetAll();

        [HttpGet]
        public IContentBlocksPreset GetPresetForPage(string documentType, string culture)
            => _presetRepository.GetPresetForPage(documentType, culture);
    }
}
