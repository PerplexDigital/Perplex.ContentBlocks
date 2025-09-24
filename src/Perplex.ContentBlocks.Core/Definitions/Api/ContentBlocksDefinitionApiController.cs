using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Api;

namespace Perplex.ContentBlocks.Definitions.Api;

public class ContentBlocksDefinitionApiController(IContentBlockDefinitionRepository definitionRepository)
    : ContentBlocksApiControllerBase
{
    [HttpGet("definitions/all")]
    public ApiContentBlockDefinition[] GetAllDefinitions()
        => definitionRepository.GetAll().Select(Map).ToArray();

    [HttpGet("definitions/forpage")]
    public ApiContentBlockDefinition[] GetDefinitionsForPage(string documentType, string? culture)
        => definitionRepository.GetAllForPage(documentType, culture).Select(Map).ToArray();

    private static ApiContentBlockDefinition Map(IContentBlockDefinition definition)
    {
        return new ApiContentBlockDefinition
        {
            Id = definition.Id,
            Name = definition.Name,
            Description = definition.Description,
            BlockNameTemplate = definition.BlockNameTemplate,
            Icon = definition.Icon,
            PreviewImage = definition.PreviewImage,
            ElementTypeKey = definition.ElementTypeKey,
            CategoryIds = definition.CategoryIds,
            Layouts = definition.Layouts.Select(Map).ToArray(),
            LimitToCultures = definition.LimitToCultures,
            LimitToDocumentTypes = definition.LimitToDocumentTypes,
        };

        static ApiContentBlockLayout Map(IContentBlockLayout layout)
        {
            return new ApiContentBlockLayout
            {
                Id = layout.Id,
                Description = layout.Description,
                Name = layout.Name,
                PreviewImage = layout.PreviewImage,
            };
        }
    }
}
