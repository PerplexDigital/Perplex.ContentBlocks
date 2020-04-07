using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Preview;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Perplex.ContentBlocks.Rendering
{
    public class ContentBlockRenderer : IContentBlockRenderer
    {
        private readonly IContentBlockDefinitionRepository _definitionRepository;
        private readonly bool _isPreview;

        public ContentBlockRenderer(IContentBlockDefinitionRepository definitionRepository, IPreviewModeProvider previewModeProvider)
        {
            _definitionRepository = definitionRepository;
            _isPreview = previewModeProvider.IsPreviewMode;
        }

        public IHtmlString Render(IContentBlocks contentBlocks, HtmlHelper htmlHelper)
        {
            if (contentBlocks.Header == null && contentBlocks.Blocks?.Any() != true)
            {
                return MvcHtmlString.Empty;
            }

            StringBuilder sb = new StringBuilder();

            IHtmlString headerHtml = RenderBlock(contentBlocks.Header, htmlHelper);
            sb.Append(headerHtml.ToString());

            foreach (var block in contentBlocks.Blocks)
            {
                IHtmlString blockHtml = RenderBlock(block, htmlHelper);
                sb.Append(blockHtml.ToString());
            }

            return new HtmlString(sb.ToString());
        }

        public IHtmlString RenderHeader(IContentBlocks contentBlocks, HtmlHelper htmlHelper)
        {
            if (contentBlocks.Header == null)
            {
                return MvcHtmlString.Empty;
            }

            return RenderBlock(contentBlocks.Header, htmlHelper);
        }

        public IHtmlString RenderBlocks(IContentBlocks contentBlocks, HtmlHelper htmlHelper)
        {
            if (contentBlocks.Blocks?.Any() != true)
            {
                return MvcHtmlString.Empty;
            }

            StringBuilder sb = new StringBuilder();

            foreach (var block in contentBlocks.Blocks)
            {
                IHtmlString blockHtml = RenderBlock(block, htmlHelper);
                sb.Append(blockHtml.ToString());
            }

            return new HtmlString(sb.ToString());
        }

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
                string blockIdAnchor = $"<a id=\"{contentBlockViewModel.Id}\"></a>";
                return new MvcHtmlString(blockIdAnchor + contentBlockHtml);
            }
        }

        private string GetViewPath(Guid definitionId, Guid layoutId)
        {
            var definition = _definitionRepository.GetById(definitionId);
            return definition
                ?.Layouts?.FirstOrDefault(l => l.Id == layoutId)
                ?.ViewPath;
        }
    }
}
