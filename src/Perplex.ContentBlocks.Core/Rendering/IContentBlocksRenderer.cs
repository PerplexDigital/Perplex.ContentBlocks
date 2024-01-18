using Microsoft.AspNetCore.Html;

namespace Perplex.ContentBlocks.Rendering;

/// <summary>
/// Renders content blocks
/// </summary>
public interface IContentBlocksRenderer
{
    /// <summary>
    /// Renders all content blocks
    /// </summary>
    /// <param name="contentBlocks">The content blocks to render</param>
    /// <param name="renderViewComponentAsync">View component render method</param>
    /// <param name="renderPartialViewAsync">Partial view render method</param>
    /// <param name="isBackOfficePreview">Indicates if the content blocks are rendered for the preview window in the backoffice</param>
    /// <returns>The rendered content blocks</returns>
    Task<IHtmlContent> RenderAsync(IContentBlocks? contentBlocks, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync, bool isBackOfficePreview = false);

    /// <summary>
    /// Renders a single content block
    /// </summary>
    /// <param name="block">The content block to render</param>
    /// <param name="renderViewComponentAsync">View component render method</param>
    /// <param name="renderPartialViewAsync">Partial view render method</param>
    /// <param name="isBackOfficePreview">Indicates if the content blocks are rendered for the preview window in the backoffice</param>
    /// <returns>The rendered content block</returns>
    Task<IHtmlContent> RenderBlockAsync(IContentBlockViewModel? block, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync, bool isBackOfficePreview = false);

    /// <summary>
    /// Renders multiple content blocks
    /// </summary>
    /// <param name="blocks">The content blocks to render</param>
    /// <param name="renderViewComponentAsync">View component render method</param>
    /// <param name="renderPartialViewAsync">Partial view render method</param>
    /// <param name="isBackOfficePreview">Indicates if the content blocks are rendered for the preview window in the backoffice</param>
    /// <returns>The rendered content blocks</returns>
    Task<IHtmlContent> RenderBlocksAsync(IEnumerable<IContentBlockViewModel>? blocks, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync, bool isBackOfficePreview = false);
}
