using Microsoft.AspNetCore.Html;
using Perplex.ContentBlocks.Preview;

namespace Perplex.ContentBlocks.Rendering;

[Obsolete($"Use {nameof(ContentBlocksRenderer)} instead.")]
public class ContentBlockRenderer : IContentBlockRenderer
{
    private readonly bool _isPreview;
    private readonly IContentBlocksRenderer _renderer;

    public ContentBlockRenderer(
        IPreviewModeProvider previewModeProvider,
        IContentBlocksRenderer renderer)
    {
        _isPreview = previewModeProvider.IsPreviewMode;
        _renderer = renderer;
    }

    private static Task<IHtmlContent> UnsupportedViewComponentRender(Type componentType, object? arguments)
        => throw new NotSupportedException($"ContentBlock definitions with view components are not supported by this renderer. Use {nameof(IContentBlocksRenderer)} instead.");

    public Task<IHtmlContent> Render(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
        => _renderer.RenderAsync(contentBlocks, UnsupportedViewComponentRender, renderPartialViewAsync, _isPreview);

    public Task<IHtmlContent> RenderBlocks(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
        => _renderer.RenderBlocksAsync(contentBlocks?.Blocks, UnsupportedViewComponentRender, renderPartialViewAsync, _isPreview);

    public Task<IHtmlContent> RenderBlocks(IEnumerable<IContentBlockViewModel>? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
        => _renderer.RenderBlocksAsync(contentBlocks, UnsupportedViewComponentRender, renderPartialViewAsync, _isPreview);

    public Task<IHtmlContent> RenderBlock(IContentBlockViewModel? contentBlockViewModel, RenderPartialViewAsync renderPartialViewAsync)
        => _renderer.RenderBlockAsync(contentBlockViewModel, UnsupportedViewComponentRender, renderPartialViewAsync, _isPreview);

    public Task<IHtmlContent> RenderHeader(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
        => RenderBlock(contentBlocks?.Header, renderPartialViewAsync);
}
