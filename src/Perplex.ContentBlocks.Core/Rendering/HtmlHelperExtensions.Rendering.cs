﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Perplex.ContentBlocks.Rendering;

[Obsolete("Use <perplex-content-blocks> tag helper instead.")]
public static partial class HtmlHelperExtensions
{
    /// <summary>
    /// Renders all Content Blocks.
    /// </summary>
    /// <param name="html">HtmlHelper</param>
    /// <param name="contentBlocks">Content Blocks to render</param>
    /// <param name="renderer">Content Blocks renderer</param>
    /// <returns></returns>
    [Obsolete("Use <perplex-content-blocks> tag helper instead.")]
    public static async Task<IHtmlContent> RenderContentBlocks(this IHtmlHelper html, IContentBlocks? contentBlocks, IContentBlockRenderer renderer)
        => await renderer.Render(contentBlocks, html.PartialAsync);

    /// <summary>
    /// Renders a single Content Block
    /// </summary>
    /// <param name="html">HtmlHelper</param>
    /// <param name="contentBlock">Content Block to render</param>
    /// <param name="renderer">Content Blocks renderer</param>
    /// <returns></returns>
    [Obsolete("Use <perplex-content-blocks> tag helper instead.")]
    public static async Task<IHtmlContent> RenderContentBlock(this IHtmlHelper html, IContentBlockViewModel contentBlock, IContentBlockRenderer renderer)
        => await renderer.RenderBlock(contentBlock, html.PartialAsync);

    /// <summary>
    /// Renders multiple Content Blocks
    /// </summary>
    /// <param name="html">HtmlHelper</param>
    /// <param name="contentBlocks">Content Blocks to render</param>
    /// <param name="renderer">Content Blocks renderer</param>
    /// <returns></returns>
    [Obsolete("Use <perplex-content-blocks> tag helper instead.")]
    public static async Task<IHtmlContent> RenderContentBlocks(this IHtmlHelper html, IEnumerable<IContentBlockViewModel> contentBlocks, IContentBlockRenderer renderer)
        => await renderer.RenderBlocks(contentBlocks, html.PartialAsync);
}
