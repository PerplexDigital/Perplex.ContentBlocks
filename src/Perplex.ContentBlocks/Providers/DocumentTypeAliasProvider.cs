using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.Providers
{
    public class DocumentTypeAliasProvider : IDocumentTypeAliasProvider
    {
        private readonly IContentService _contentService;

        public DocumentTypeAliasProvider(IContentService contentService)
        {
            _contentService = contentService;
        }

        public string GetDocumentTypeAlias(int pageId)
        {
            IContent content = _contentService.GetById(pageId);
            if (content == null)
            {
                return null;
            }

            return content.ContentType?.Alias;
        }
    }
}
