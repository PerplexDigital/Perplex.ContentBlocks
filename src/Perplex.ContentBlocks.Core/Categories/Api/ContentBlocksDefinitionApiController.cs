using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Api;

namespace Perplex.ContentBlocks.Categories.Api;

[ApiExplorerSettings(GroupName = "Categories")]
public class ContentBlocksCategoriesApiController
(
    IContentBlockCategoryRepository categoryRepository
) : ContentBlocksApiControllerBase
{
    [HttpGet("categories/all")]
    public ApiContentBlockCategory[] GetAllCategories()
        => categoryRepository.GetAll(true).Select(Map).ToArray();

    private static ApiContentBlockCategory Map(IContentBlockCategory category)
    {
        return new ApiContentBlockCategory
        {
            Id = category.Id,
            IsEnabledForHeaders = category.IsEnabledForHeaders,
            Icon = category.Icon,
            IsDisabledForBlocks = category.IsDisabledForBlocks,
            IsHidden = category.IsHidden,
            Name = category.Name,
        };
    }
}
