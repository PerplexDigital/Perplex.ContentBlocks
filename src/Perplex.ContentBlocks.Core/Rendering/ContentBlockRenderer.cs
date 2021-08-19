using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Preview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET5_0
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#elif NET472
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace Perplex.ContentBlocks.Rendering
{
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

#if NET5_0

        public async Task<IHtmlContent> Render(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
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

        public async Task<IHtmlContent> RenderBlocks(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
            => await RenderBlocks(contentBlocks?.Blocks, renderPartialViewAsync);

        public async Task<IHtmlContent> RenderBlocks(IEnumerable<IContentBlockViewModel> contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
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

        public async Task<IHtmlContent> RenderBlock(IContentBlockViewModel contentBlockViewModel, RenderPartialViewAsync renderPartialViewAsync)
        {
            if (contentBlockViewModel == null)
            {
                return HtmlString.Empty;
            }

            string viewPath = GetViewPath(contentBlockViewModel.DefinitionId, contentBlockViewModel.LayoutId);
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

        public async Task<IHtmlContent> RenderHeader(IContentBlocks contentBlocks, RenderPartialViewAsync renderPartialViewAsync)
            => await RenderBlock(contentBlocks?.Header, renderPartialViewAsync);

#elif NET472

        public IHtmlString Render(IContentBlocks contentBlocks, HtmlHelper htmlHelper)
        {
            if (contentBlocks == null)
            {
                return MvcHtmlString.Empty;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(RenderHeader(contentBlocks, htmlHelper));
            sb.Append(RenderBlocks(contentBlocks, htmlHelper));

            return new HtmlString(sb.ToString());
        }

        public IHtmlString RenderHeader(IContentBlocks contentBlocks, HtmlHelper htmlHelper)
        {
            if (contentBlocks?.Header == null)
            {
                return MvcHtmlString.Empty;
            }

            return RenderBlock(contentBlocks.Header, htmlHelper);
        }

        public IHtmlString RenderBlocks(IContentBlocks contentBlocks, HtmlHelper htmlHelper)
            => RenderBlocks(contentBlocks?.Blocks, htmlHelper);

        public IHtmlString RenderBlock(IContentBlockViewModel contentBlockViewModel, HtmlHelper htmlHelper)
        {
            if (contentBlockViewModel == null)
            {
                return MvcHtmlString.Empty;
            }

            string viewPath = GetViewPath(contentBlockViewModel.DefinitionId, contentBlockViewModel.LayoutId);
            if (string.IsNullOrEmpty(viewPath))
            {
                return MvcHtmlString.Empty;
            }

            IHtmlString contentBlockHtml = htmlHelper.Partial(viewPath, contentBlockViewModel);

            if (!_isPreview)
            {
                return contentBlockHtml;
            }
            else
            {
                // Preview mode: add block id for scroll synchronisation
                string blockIdAnchor = $"<a id=\"{contentBlockViewModel.Id}\" class=\"perplex-content-blocks-preview-anchor\"></a>";
                return new MvcHtmlString(blockIdAnchor + contentBlockHtml);
            }
        }

        public IHtmlString RenderBlocks(IEnumerable<IContentBlockViewModel> contentBlocks, HtmlHelper htmlHelper)
        {
            if (contentBlocks?.Any() != true)
            {
                return MvcHtmlString.Empty;
            }

            StringBuilder sb = new StringBuilder();

            foreach (var block in contentBlocks)
            {
                IHtmlString blockHtml = RenderBlock(block, htmlHelper);
                sb.Append(blockHtml.ToString());
            }

            return new HtmlString(sb.ToString());
        }

#endif

        private string GetViewPath(Guid definitionId, Guid layoutId)
        {
            var definition = _definitionRepository.GetById(definitionId);
            return definition
                ?.Layouts?.FirstOrDefault(l => l.Id == layoutId)
                ?.ViewPath;
        }
    }
}
