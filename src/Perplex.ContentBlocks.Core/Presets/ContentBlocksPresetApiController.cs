using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Api;
using Perplex.ContentBlocks.Presets;

namespace Perplex.ContentBlocks.Definitions;

public class ContentBlocksPresetApiController : ContentBlocksApiControllerBase
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
