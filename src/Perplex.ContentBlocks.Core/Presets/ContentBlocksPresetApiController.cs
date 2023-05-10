using Perplex.ContentBlocks.Presets;
using System.Collections.Generic;
#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
#elif NETFRAMEWORK
using System.Web.Http;
using Umbraco.Web.WebApi;
#endif

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
