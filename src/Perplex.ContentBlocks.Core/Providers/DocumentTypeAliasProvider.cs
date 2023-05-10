#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
#elif NETFRAMEWORK
using Umbraco.Core.Models;
using Umbraco.Core.Services;
#endif


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
