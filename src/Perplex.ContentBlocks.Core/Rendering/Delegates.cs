using Microsoft.AspNetCore.Html;

namespace Perplex.ContentBlocks.Rendering;

/// <summary>
/// Renders a partial view with the given model
/// </summary>
/// <param name="partialViewName">Name of the partial view</param>
/// <param name="model">Model to pass to the partial view</param>
/// <returns>The rendered partial view</returns>
public delegate Task<IHtmlContent> RenderPartialViewAsync(string partialViewName, object model);

/// <summary>
/// Renders a view component with the given arguments
/// </summary>
/// <param name="componentType">The <see cref="Type"/> of the view component</param>
/// <param name="arguments">Arguments to pass to the view component</param>
/// <returns>The rendered view component</returns>
public delegate Task<IHtmlContent> RenderViewComponentAsync(Type componentType, object? arguments);
