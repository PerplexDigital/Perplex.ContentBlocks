using Microsoft.AspNetCore.Html;
using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Preview;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlockRenderer : IContentBlockRenderer
{
    private readonly IContentBlockDefinitionRepository _definitionRepository;
    private readonly bool _isPreview;

    public ContentBlockRenderer(
        IContentBlockDefinitionRepository definitionRepository,
        IPreviewModeProvider previewModeProvider)
    {
        _definitionRepository = definitionRepository;
        _isPreview = previewModeProvider.IsPreviewMode;
    }

    public async Task<IHtmlContent> Render(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
    {
        if (contentBlocks == null)
        {
            return HtmlString.Empty;
        }

        var builder = new HtmlContentBuilder();

        var blocksHtml = await Task.WhenAll(
            RenderHeader(contentBlocks, renderPartialViewAsync),
            RenderBlocks(contentBlocks, renderPartialViewAsync)
        );

        foreach (var blockHtml in blocksHtml)
        {
            builder.AppendHtml(blockHtml);
        }

        return builder;
    }

    public async Task<IHtmlContent> RenderBlocks(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
        => await RenderBlocks(contentBlocks?.Blocks, renderPartialViewAsync);

    public async Task<IHtmlContent> RenderBlocks(IEnumerable<IContentBlockViewModel>? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
    {
        if (contentBlocks?.Any() != true)
        {
            return HtmlString.Empty;
        }

        var builder = new HtmlContentBuilder();

        var blocksHtml = await Task.WhenAll(contentBlocks.Select(block => RenderBlock(block, renderPartialViewAsync)));

        foreach (var blockHtml in blocksHtml)
        {
            builder.AppendHtml(blockHtml);
        }

        return builder;
    }

    public async Task<IHtmlContent> RenderBlock(IContentBlockViewModel? contentBlockViewModel, RenderPartialViewAsync renderPartialViewAsync)
    {
        if (contentBlockViewModel == null)
        {
            return HtmlString.Empty;
        }

        var viewPath = GetViewPath(contentBlockViewModel.DefinitionId, contentBlockViewModel.LayoutId);
        if (string.IsNullOrEmpty(viewPath))
        {
            return HtmlString.Empty;
        }

        IHtmlContent contentBlockHtml = await renderPartialViewAsync(viewPath, contentBlockViewModel);

        var builder = new HtmlContentBuilder();

        if (_isPreview)
        {
            // Preview mode: add block id for scroll synchronisation
            string blockIdAnchor = $"<a id=\"{contentBlockViewModel.Id}\" class=\"perplex-content-blocks-preview-anchor\"></a>";
            builder.AppendHtml(blockIdAnchor);
        }

        builder.AppendHtml(contentBlockHtml);

        return builder;
    }

    public async Task<IHtmlContent> RenderHeader(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
        => await RenderBlock(contentBlocks?.Header, renderPartialViewAsync);

    private string? GetViewPath(Guid definitionId, Guid layoutId)
    {
        var definition = _definitionRepository.GetById(definitionId);
        return definition
            ?.Layouts?.FirstOrDefault(l => l.Id == layoutId)
            ?.ViewPath;
    }
}
