using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Api;
using System.Diagnostics.CodeAnalysis;

namespace Perplex.ContentBlocks.Presets.Api;

public class ContentBlocksPresetApiController(IContentBlocksPresetRepository presetRepository) : ContentBlocksApiControllerBase
{
    [HttpGet("presets/all")]
    public ApiContentBlocksPreset[] GetAllPresets()
        => presetRepository.GetAll().Select(Map).OfType<ApiContentBlocksPreset>().ToArray();

    [HttpGet("presets/forpage")]
    public ApiContentBlocksPreset? GetPresetForPage(string documentType, string? culture)
        => Map(presetRepository.GetPresetForPage(documentType, culture));

    [return: NotNullIfNotNull(nameof(preset))]
    private static ApiContentBlocksPreset? Map(IContentBlocksPreset? preset)
    {
        if (preset is null)
        {
            return null;
        }

        return new ApiContentBlocksPreset
        {
            ApplyToCultures = preset.ApplyToCultures.ToArray(),
            ApplyToDocumentTypes = preset.ApplyToDocumentTypes.ToArray(),
            Blocks = preset.Blocks.Select(Map).OfType<ApiContentBlockPreset>().ToArray(),
            Id = preset.Id,
            Header = Map(preset.Header),
            Name = preset.Name
        };
    }

    [return: NotNullIfNotNull(nameof(preset))]
    private static ApiContentBlockPreset? Map(IContentBlockPreset? preset)
    {
        if (preset is null)
        {
            return null;
        }

        return new ApiContentBlockPreset
        {
            DefinitionId = preset.DefinitionId,
            Id = preset.Id,
            IsMandatory = preset.IsMandatory,
            LayoutId = preset.LayoutId,
            Values = preset.Values,
            Variants = preset.Variants.Select(Map).ToArray()
        };
    }

    private static ApiContentBlockVariantPreset Map(IContentBlockVariantPreset preset)
    {
        return new ApiContentBlockVariantPreset
        {
            Alias = preset.Alias,
            Id = preset.Id,
            Values = preset.Values
        };
    }
}
