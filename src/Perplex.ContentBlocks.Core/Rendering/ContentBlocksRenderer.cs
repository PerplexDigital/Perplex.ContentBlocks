using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
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

        if (contentBlocks.Header is not null)
        {
            var headerHtml = await RenderBlockAsync(contentBlocks.Header, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview);
            builder.AppendHtml(headerHtml);
        }

        if (contentBlocks.Blocks?.Any() == true)
        {
            var blocksHtml = await RenderBlocksAsync(contentBlocks.Blocks, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview);
            builder.AppendHtml(blocksHtml);
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

        foreach (var block in blocks)
        {
            var blockHtml = await RenderBlockAsync(block, renderViewComponentAsync, renderPartialViewAsync, isBackOfficePreview);
            builder.AppendHtml(blockHtml);
        }

        return builder;
    }

    private static async Task<IHtmlContent> GetBlockHtml(IContentBlockViewModel block, IContentBlockDefinition definition, RenderViewComponentAsync renderViewComponentAsync, RenderPartialViewAsync renderPartialViewAsync)
    {
        if (definition is IContentBlockDefinition<ViewComponent> componentDefinition)
        {
            Type viewComponentType = componentDefinition.GetType().GenericTypeArguments[0];
            return await renderViewComponentAsync(viewComponentType, block);
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
