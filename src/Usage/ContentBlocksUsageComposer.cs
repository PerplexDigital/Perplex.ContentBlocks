using Athlon.Features.Migrations;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Usage
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(ContentBlocksMigrationsComposer))]
    public class ContentBlocksUsageComposer : ComponentComposer<ContentBlocksUsageComponent>, IUserComposer
    {
        public override void Compose(Composition composition)
        {
            base.Compose(composition);

            composition.Register<IContentBlockUsageRepository, ContentBlocksUsageDatabaseRepository>();
            composition.Register<IContentBlockUsageService, ContentBlockUsageService>();
            composition.Register<IContentBlockUsageAnalyzer, ContentBlockUsageAnalyzer>();
            composition.Register<IContentBlockUsageWebsiteIdProvider, ContentBlockUsageHomepageWebsiteIdProvider>();
        }
    }
}
