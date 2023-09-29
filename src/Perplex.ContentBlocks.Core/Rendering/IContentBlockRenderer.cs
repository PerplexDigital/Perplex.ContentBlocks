using Microsoft.AspNetCore.Html;

namespace Perplex.ContentBlocks.Rendering;

public interface IContentBlockRenderer
{
    Task<IHtmlContent> Render(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

    Task<IHtmlContent> RenderHeader(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

    Task<IHtmlContent> RenderBlocks(IContentBlocks? contentBlocks, RenderPartialViewAsync renderPartialViewAsync);

    Task<IHtmlContent> RenderBlock(IContentBlockViewModel? contentBlockViewModel, RenderPartialViewAsync renderPartialViewAsync);

    Task<IHtmlContent> RenderBlocks(IEnumerable<IContentBlockViewModel>? contentBlocks, RenderPartialViewAsync renderPartialViewAsync);
}
