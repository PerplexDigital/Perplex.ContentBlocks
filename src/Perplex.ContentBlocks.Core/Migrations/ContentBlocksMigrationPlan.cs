using Perplex.ContentBlocks.Migrations.M001_MigrateFromNestedContentToBlockList;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Perplex.ContentBlocks.Migrations;

public class ContentBlocksMigrationPlan : MigrationPlan
{
    public override string InitialState => string.Empty;

    public ContentBlocksMigrationPlan() : base("Perplex.ContentBlocks")
    {
        From(InitialState);

        To<MigrateFromNestedContentToBlockList>("MigrateFromNestedContentToBlockList");
    }
}
