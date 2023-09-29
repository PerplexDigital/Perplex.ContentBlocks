using Microsoft.AspNetCore.Html;
using Perplex.ContentBlocks.Definitions;

namespace Perplex.ContentBlocks.Rendering;

public class ContentBlocksRenderer : IContentBlocksRenderer
{
    private readonly IContentBlockDefinitionRepository _definitionRepository;

    public ContentBlocksRenderer(IContentBlockDefinitionRepository definitionRepository)
    {
        _definitionRepository = definitionRepository;
    }

    /// <inheritdoc/>
    public async Task<IHtmlContent> RenderAsync(IContentBlocks? contentBlocks, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync, bool isBackOfficePreview = false)
    {
        if (contentBlocks is null)
        {
            return HtmlString.Empty;
        }

        var builder = new HtmlContentBuilder();

        var blocksHtml = await Task.WhenAll(
            RenderBlockAsync(contentBlocks.Header, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview),
            RenderBlocksAsync(contentBlocks.Blocks, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview)
        );

        foreach (var blockHtml in blocksHtml)
        {
            builder.AppendHtml(blockHtml);
        }

        return builder;
    }

    /// <inheritdoc/>
    public async Task<IHtmlContent> RenderBlockAsync(IContentBlockViewModel? block, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync, bool isBackOfficePreview = false)
    {
        if (block is null ||
            _definitionRepository.GetById(block.DefinitionId) is not IContentBlockDefinition definition)
        {
            return HtmlString.Empty;
        }

        var blockHtml = await GetBlockHtml(block, definition, renderViewComponentAsync, renderPartialViewAsync);

        if (isBackOfficePreview)
        {
            // Preview mode: add block id for scroll synchronisation before the block html
            var blockIdAnchor = $"<a id=\"{block.Id}\" class=\"perplex-content-blocks-preview-anchor\"></a>";
            return new HtmlContentBuilder()
                .AppendHtml(blockIdAnchor)
                .AppendHtml(blockHtml);
        }
        else
        {
            // No preview mode: block html only
            return blockHtml;
        }
    }

    /// <inheritdoc/>
    public async Task<IHtmlContent> RenderBlocksAsync(IEnumerable<IContentBlockViewModel>? blocks, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync, bool isBackOfficePreview = false)
    {
        if (blocks?.Any() != true)
        {
            return HtmlString.Empty;
        }

        var builder = new HtmlContentBuilder();

        var renderTasks = blocks.Select(block => RenderBlockAsync(block, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview));
        var blocksHtml = await Task.WhenAll(renderTasks);

        foreach (var blockHtml in blocksHtml)
        {
            builder.AppendHtml(blockHtml);
        }

        return builder;
    }

    private static async Task<IHtmlContent> GetBlockHtml(IContentBlockViewModel block, IContentBlockDefinition definition, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync)
    {
        if (definition is IContentBlockDefinition<IContentBlockViewComponent> componentDefinition)
        {
            Type viewComponentType = componentDefinition.GetType().GenericTypeArguments[0];
            return await renderViewComponentAsync(viewComponentType, new { model = block });
        }
        else
        {
            var viewPath = definition.Layouts.FirstOrDefault(l => l.Id == block.LayoutId)?.ViewPath;
            if (string.IsNullOrEmpty(viewPath))
            {
                return HtmlString.Empty;
            }

            return await renderPartialViewAsync(viewPath, block);
        }
    }
}
