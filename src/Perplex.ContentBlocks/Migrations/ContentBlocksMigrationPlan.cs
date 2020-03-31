using Perplex.ContentBlocks.Migrations.v_1_0_0;
using Umbraco.Core.Migrations;

namespace Perplex.ContentBlocks.Features.Migrations
{
    public class ContentBlocksMigrationPlan : MigrationPlan
    {
        public ContentBlocksMigrationPlan() : base("Perplex.ContentBlocks")
        {
            Setup();
        }

        private void Setup()
        {
            From(InitialState);

            To<AddContentBlocksDataType>("AddContentBlocksDataType");
        }
    }
}
