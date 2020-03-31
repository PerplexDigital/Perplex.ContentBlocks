using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlocksUsageComponent : IComponent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IContentBlockUsageAnalyzer _analyzer;
        private readonly IContentBlockUsageRepository _repository;

        public ContentBlocksUsageComponent(
            IUmbracoContextFactory umbracoContextFactory,
            IContentBlockUsageAnalyzer analyzer,
            IContentBlockUsageRepository repository)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _analyzer = analyzer;
            _repository = repository;
        }

        public void Initialize()
        {
            UpdateAllUses();

            ContentService.Published += ContentService_Published;
            ContentService.Deleted += ContentService_Deleted;
        }

        private void ContentService_Deleted(IContentService sender, DeleteEventArgs<IContent> e)
        {
            foreach (var entity in e.DeletedEntities)
            {
                _repository.ClearPage(entity.Key);
            }
        }

        private void UpdateAllUses()
        {
            var allUses = _analyzer.GetAllUses().ToList();

            if (allUses.Count > 0)
            {
                _repository.WithLock(() =>
                {
                    _repository.Clear();
                    _repository.Save(allUses);
                });
            }
        }

        private void ContentService_Published(IContentService sender, ContentPublishedEventArgs e)
        {
            using (var reference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var entity in e.PublishedEntities)
                {
                    var content = reference.UmbracoContext.Content.GetById(entity.Id);

                    if (content is IPublishedContent page)
                    {
                        foreach (var cultureInfo in entity.CultureInfos)
                        {
                            var uses = _analyzer.GetUsesForPage(page, cultureInfo.Culture);

                            _repository.WithLock(() =>
                            {
                                _repository.ClearPage(page.Key, cultureInfo.Culture);
                                _repository.Save(uses);
                            });
                        }
                    }
                }
            }
        }

        public void Terminate()
        {
        }
    }
}
