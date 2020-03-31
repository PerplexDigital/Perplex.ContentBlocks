using System;
using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlocksUsageApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentBlockUsageService _usageService;
        private readonly IContentBlockUsageWebsiteIdProvider _websiteIdProvider;

        public ContentBlocksUsageApiController(IContentBlockUsageService usageService, IContentBlockUsageWebsiteIdProvider websiteIdProvider)
        {
            _usageService = usageService;
            _websiteIdProvider = websiteIdProvider;
        }

        [HttpGet]
        public IDictionary<Guid, IContentBlockUsage> GetAllContentBlockUses(int? websiteId, string culture)
            => _usageService.GetAll(websiteId, culture);

        [HttpGet]
        public int? GetWebsiteId(int pageId)
            => _websiteIdProvider.GetWebsiteId(pageId);
    }
}
