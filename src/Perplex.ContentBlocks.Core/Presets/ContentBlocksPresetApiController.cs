using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Presets;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Perplex.ContentBlocks.Definitions;

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
    public IContentBlocksPreset? GetPresetForPage(string documentType, string culture)
        => _presetRepository.GetPresetForPage(documentType, culture);
}
