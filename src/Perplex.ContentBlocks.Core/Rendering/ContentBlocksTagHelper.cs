using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Perplex.ContentBlocks.Preview;
using System.Reflection;

namespace Perplex.ContentBlocks.Rendering;

/// <summary>
/// Tag helper to render ContentBlocks
/// </summary>
[HtmlTargetElement("perplex-content-blocks", TagStructure = TagStructure.WithoutEndTag)]
public class ContentBlocksTagHelper : TagHelper
{
    private readonly IViewComponentHelper _viewComponentHelper;
    private readonly IHtmlHelper _htmlHelper;
    private readonly IPreviewModeProvider _previewModeProvider;
    private readonly IContentBlocksRenderer _renderer;

    public ContentBlocksTagHelper(
        IViewComponentHelper viewComponentHelper,
        IHtmlHelper htmlHelper,
        IPreviewModeProvider previewModeProvider,
        IContentBlocksRenderer renderer)
    {
        _viewComponentHelper = viewComponentHelper;
        _htmlHelper = htmlHelper;
        _previewModeProvider = previewModeProvider;
        _renderer = renderer;
    }

    /// <summary>
    /// The <see cref="IContentBlocks"/> content to render
    /// </summary>
    public IContentBlocks? Content { get; set; }

    /// <summary>
    /// A single ContentBlock to render
    /// </summary>
    public IContentBlockViewModel? Block { get; set; }

    /// <summary>
    /// Multiple ContentBlocks to render
    /// </summary>
    public IEnumerable<IContentBlockViewModel>? Blocks { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var blocks = GetBlocks().ToArray();

        if (blocks.Length == 0 || ViewContext is null)
        {
            return;
        }

        EnsureViewContext(_viewComponentHelper, ViewContext);
        EnsureViewContext(_htmlHelper, ViewContext);

        var html = await _renderer.RenderBlocksAsync(
            blocks,
            _viewComponentHelper.InvokeAsync,
            _htmlHelper.PartialAsync,
            _previewModeProvider.IsPreviewMode);

        output.Content.SetHtmlContent(html);
    }

    private IEnumerable<IContentBlockViewModel> GetBlocks()
    {
        if (Content?.Header is not null)
        {
            yield return Content.Header;
        }

        if (Content?.Blocks is not null)
        {
            foreach (var block in Content.Blocks)
            {
                yield return block;
            }
        }

        if (Block is not null)
        {
            yield return Block;
        }

        if (Blocks is not null)
        {
            foreach (var block in Blocks)
            {
                yield return block;
            }
        }
    }

    private static void EnsureViewContext(IViewComponentHelper viewComponentHelper, ViewContext viewContext)
    {
        if (viewComponentHelper is DefaultViewComponentHelper defaultViewComponentHelper)
        {
            // Default case
            defaultViewComponentHelper.Contextualize(viewContext);
            return;
        }

        // Some other IViewComponentHelper: attempt to call a Contextualize method
        TryCallContextualize(viewComponentHelper, viewContext);
    }

    private static void EnsureViewContext(IHtmlHelper htmlHelper, ViewContext viewContext)
    {
        if (htmlHelper is HtmlHelper defaultHtmlHelper)
        {
            // Default case
            defaultHtmlHelper.Contextualize(viewContext);
            return;
        }

        // Some other IHtmlHelper: attempt to call a Contextualize method
        TryCallContextualize(htmlHelper, viewContext);
    }

    private static void TryCallContextualize(object instance, ViewContext viewContext)
    {
        Type[] parameterTypes = new[] { typeof(ViewContext) };
        MethodInfo? methodInfo = instance.GetType().GetMethod("Contextualize", parameterTypes);
        methodInfo?.Invoke(instance, new[] { viewContext });
    }
}
