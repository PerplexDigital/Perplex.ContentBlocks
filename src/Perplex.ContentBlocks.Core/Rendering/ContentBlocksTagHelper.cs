using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Perplex.ContentBlocks.Preview;
using System.Reflection;

namespace Perplex.ContentBlocks.Rendering;

[HtmlTargetElement("perplex-content-blocks", Attributes = "content", TagStructure = TagStructure.WithoutEndTag)]
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

    public IContentBlocks? Content { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (Content is null || ViewContext is null)
        {
            return;
        }

        EnsureViewContext(_viewComponentHelper, ViewContext);
        EnsureViewContext(_htmlHelper, ViewContext);

        RenderViewComponentAsync renderViewComponentAsync = _viewComponentHelper.InvokeAsync;
        RenderPartialViewAsync renderPartialViewAsync = _htmlHelper.PartialAsync;

        var isBackOfficePreview = _previewModeProvider.IsPreviewMode;

        var htmlContent = await Task.WhenAll(
            _renderer.RenderBlockAsync(Content.Header, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview),
            _renderer.RenderBlocksAsync(Content.Blocks, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview)
        );

        foreach (var html in htmlContent)
        {
            output.Content.AppendHtml(html);
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

        // Some other implementation of IViewComponentHelper,
        // use reflection and look for a _viewContext field to set.

        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        var fieldInfo = viewComponentHelper.GetType().GetField("_viewContext", flags);

        if (fieldInfo is not null &&
            fieldInfo.GetValue(viewComponentHelper) is null)
        {
            fieldInfo.SetValue(viewComponentHelper, viewContext);
        }
    }

    private static void EnsureViewContext(IHtmlHelper htmlHelper, ViewContext viewContext)
    {
        if (htmlHelper is HtmlHelper hh)
        {
            // Default case
            hh.Contextualize(viewContext);
            return;
        }

        // Some other IHtmlHelper: use reflection
        Type[] parameterTypes = new[] { typeof(ViewContext) };
        MethodInfo? methodInfo = htmlHelper.GetType().GetMethod("Contextualize", parameterTypes);
        methodInfo?.Invoke(htmlHelper, new[] { viewContext });
    }
}
