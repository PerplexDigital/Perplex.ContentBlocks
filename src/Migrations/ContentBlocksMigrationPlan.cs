using Umbraco.Core.Migrations;

namespace Perplex.ContentBlocks.Features.Migrations
{
    public class ContentBlocksMigrationPlan : MigrationPlan
    {
        public ContentBlocksMigrationPlan() : base("ContentBlocks")
        {
            Setup();
        }

        private void Setup()
        {
            From(InitialState);

            To<ContentBlocksMigrationContentBlockUsageDatabaseSetup>("35482338-f8d4-4e21-8584-78e4b32d27ad - Database Setup");

            To<ContentBlocksMigrationContentBlockUsageAddWebsiteIdColumn>("93fe1f87-a287-407a-bbfa-df34964a3431 - Usage Add Website Id");
        }
    }
}
