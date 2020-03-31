using Athlon.Infrastructure.ModelsBuilder;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlockUsageHomepageWebsiteIdProvider : IContentBlockUsageWebsiteIdProvider
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public ContentBlockUsageHomepageWebsiteIdProvider(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public int? GetWebsiteId(int pageId)
        {
            using (var reference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (reference?.UmbracoContext is UmbracoContext umbCtx)
                {
                    return umbCtx.Content?.GetById(pageId)?.AncestorOrSelf<Homepage>()?.Id;
                }
            }

            return null;
        }
    }
}
