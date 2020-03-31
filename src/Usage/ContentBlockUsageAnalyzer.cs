using Athlon.Umbraco;
using Athlon.Infrastructure.ModelsBuilder;
using Examine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using static Athlon.Constants.ContentBlocks.Umbraco;
using static Umbraco.Core.Constants;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlockUsageAnalyzer : IContentBlockUsageAnalyzer
    {
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ILocalizationService _localizationService;

        public ContentBlockUsageAnalyzer(IExamineManager examineManager, IUmbracoContextFactory umbracoContextFactory, ILocalizationService localizationService)
        {
            _examineManager = examineManager;
            _umbracoContextFactory = umbracoContextFactory;
            _localizationService = localizationService;
        }

        public IEnumerable<IContentBlockUsage> GetAllUses()
        {
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (context?.UmbracoContext?.Content is IPublishedContentCache contentCache &&
                    _examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex index) &&
                    index.GetSearcher() is ISearcher searcher)
                {
                    IEnumerable<string> cultures = _localizationService.GetAllLanguages().Select(l => l.IsoCode);
                    IEnumerable<string> fields = cultures.Select(culture => ContentBlocksPageContentBlocksAlias + "_" + culture.ToLower());

                    var query = searcher.CreateQuery().GroupedOr(fields, nameof(ContentBlockModelValue.DefinitionId));
                    var allPages = query.Execute(maxResults: 1024 * 1024).ToList();

                    return allPages
                        .Select(p => contentCache.GetById(int.Parse(p.Id)))
                        .Where(page => page?.Cultures?.Values?.Any() == true)
                        .SelectMany(page =>
                        {
                            IEnumerable<string> pageCultures = page.Cultures.Values.Select(c => c.Culture);
                            return pageCultures.SelectMany(culture => GetUsesForPage(page, culture));
                        })
                        .ToList();
                }
            }

            return Enumerable.Empty<IContentBlockUsage>();
        }

        public IEnumerable<IContentBlockUsage> GetUsesForPage(IPublishedContent page, string culture)
        {
            int? websiteId = GetWebsiteId(page);
            if (websiteId == null)
            {
                return Enumerable.Empty<IContentBlockUsage>();
            }

            return GetContentBlockDefinitionIds(page, culture)
                .GroupBy(id => id)
                .Select(g => new ContentBlockUsage
                {
                    ContentBlockDefinitionId = g.Key,
                    PageUses = new[]
                    {
                        new ContentBlockPageUsage
                        {
                            ContentBlockDefinitionId = g.Key,
                            PageId = page.Key,
                            WebsiteId = websiteId.Value,
                            Culture = culture,
                            UsageAmount = g.Count()
                        }
                    }
                });
        }

        private int? GetWebsiteId(IPublishedContent page)
        {
            // Website Id is geimplementeerd als het id van de bijbehorende homepage.
            return page.AncestorOrSelf<Homepage>()?.Id;
        }

        private IEnumerable<Guid> GetContentBlockDefinitionIds(IPublishedContent page, string culture)
        {
            if (!page.HasProperty(ContentBlocksPageContentBlocksAlias))
            {
                yield break;
            }

            // Helaas kunnen we geen properties van de IContentBlocksPage gebruiken
            // want dan gooit Umbraco de "cannot create scoped instance without a scope"-exception.
            // Daarom via de raw JSON / ContentBlocksModelValue. Werkt op zich ook wel prima maar toch jammer.
            string json = page.GetProperty(ContentBlocksPageContentBlocksAlias)?.GetSourceValue(culture)?.ToString();
            if (string.IsNullOrEmpty(json))
            {
                yield break;
            }

            ContentBlocksModelValue modelValue = JsonConvert.DeserializeObject<ContentBlocksModelValue>(json);
            if (modelValue == null)
            {
                yield break;
            }

            if (modelValue.Header?.DefinitionId is Guid headerDefinitionId)
            {
                yield return headerDefinitionId;
            }

            foreach (var block in modelValue.Blocks)
            {
                if (block?.DefinitionId is Guid definitionId)
                {
                    yield return definitionId;
                }
            }
        }
    }
}
