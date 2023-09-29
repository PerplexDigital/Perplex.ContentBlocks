using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Rendering;

namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockViewComponent
{
    Task<IViewComponentResult> InvokeAsync(IContentBlockViewModel model);
}
