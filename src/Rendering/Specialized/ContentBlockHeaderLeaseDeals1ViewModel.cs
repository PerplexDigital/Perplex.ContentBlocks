using Athlon.Definitions;
using Athlon.Extensions;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockHeaderLeaseDeals1ViewModel : ContentBlockViewModel<ContentBlockHeaderLeaseDeals1>
    {
        public IEnumerable<LeaseDealHeaderItem> HeaderItems => _headerItems.Value;

        //Headeritems zijn lazy, omdat het zelf ook alle contentblokken nodig heeft
        //wanneer we dat tijdens initializatie doen krijgen we een overflow omdat het
        //zichzelf eeuwig blijft aanmaken
        private readonly Lazy<IEnumerable<LeaseDealHeaderItem>> _headerItems;
        private readonly IContentBlockDefinitionService _contentBlockDefinitionService;
        private readonly ICurrentPublishedContentAccessor _currentPublishedContentAccessor;

        public ContentBlockHeaderLeaseDeals1ViewModel(IContentBlockDefinitionService contentBlockDefinitionService, ICurrentPublishedContentAccessor currentPublishedContentAccessor, ContentBlockHeaderLeaseDeals1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            _contentBlockDefinitionService = contentBlockDefinitionService;
            _currentPublishedContentAccessor = currentPublishedContentAccessor;

            _headerItems = new Lazy<IEnumerable<LeaseDealHeaderItem>>(GetHeaderItems);
        }

        private IEnumerable<LeaseDealHeaderItem> GetHeaderItems()
        {
            var currentPage = _currentPublishedContentAccessor?.Content as IContentBlocksPage;

            if (currentPage == null)
            {
                return new List<LeaseDealHeaderItem>();
            }

            var leaseDealsBlock = currentPage.ContentBlocks.Blocks
                .FirstOrDefault(x => x.DefinitionId.ToString().Equals(Constants.ContentBlocks.Umbraco.ContentBlocksLeaseDealsGuid, StringComparison.InvariantCultureIgnoreCase))?
                .Content as ContentBlockLeaseDeals1;
            if (leaseDealsBlock == null)
            {
                return new List<LeaseDealHeaderItem>();
            }

            return leaseDealsBlock.Categories.Select(x => new LeaseDealHeaderItem()
            {
                Title = x.Title,
                Subtitle = x.Subtitle,
                FullAnchorLink = $"{currentPage.Url}#{x.Title.ToLower().Replace(" ", "-")}",
                AnchorLink = x.Title.ToLower().Replace(" ", " - ")
            });
        }
    }

    public class LeaseDealHeaderItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string AnchorLink { get; set; }
        public string FullAnchorLink { get; set; }
    }
}