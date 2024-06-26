using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Api;
using Perplex.ContentBlocks.Categories;

namespace Perplex.ContentBlocks.Definitions.Api;

public class ContentBlocksDefinitionApiController(IContentBlockDefinitionRepository definitionRepository, IContentBlockCategoryRepository categoryRepository)
    : ContentBlocksApiControllerBase
{
    [HttpGet("definitions/all")]
    public ApiContentBlockDefinition[] GetAllDefinitions()
        => definitionRepository.GetAll().Select(Map).ToArray();

    [HttpGet("categories/all")]
    public IActionResult GetAllCategories()
    {
        var categories = categoryRepository.GetAll(true);
        return Ok(categories);
    }

    [HttpGet("definitions/forpage")]
    public IActionResult GetDefinitionsForPage(string documentType, string culture)
    {
        var definitions = definitionRepository.GetAllForPage(documentType, culture);
        return Ok(definitions);
    }

    private static ApiContentBlockDefinition Map(IContentBlockDefinition definition)
    {
        return new ApiContentBlockDefinition
        {
            Id = definition.Id,
            Name = definition.Name,
            Description = definition.Description,
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
