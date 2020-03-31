using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;
using static Athlon.Constants.ContentBlocks.Usage;

namespace Perplex.ContentBlocks.Features.Migrations
{
    public class ContentBlocksMigrationContentBlockUsageAddWebsiteIdColumn : MigrationBase
    {
        private const string WebsiteIdColumnName = "WebsiteId";

        private readonly IScopeProvider _scopeProvider;

        public ContentBlocksMigrationContentBlockUsageAddWebsiteIdColumn(IMigrationContext context, IScopeProvider scopeProvider)
            : base(context)
        {
            _scopeProvider = scopeProvider;
        }

        public override void Migrate()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                AddWebsiteIdColumn();
                AddWebsiteIdIndex();
                AlterUserDefinedTableType(scope.Database);

                scope.Complete();
            }
        }

        private void AddWebsiteIdColumn()
        {
            if (!ColumnExists(DatabaseTableName, WebsiteIdColumnName))
            {
                Alter.Table(DatabaseTableName)
                    .AddColumn(WebsiteIdColumnName)
                    .AsInt32()
                    .NotNullable()
                    .Do();
            }
        }

        private void AddWebsiteIdIndex()
        {
            string indexName = $"IX_{DatabaseTableName}_{WebsiteIdColumnName}";

            if (!IndexExists(indexName))
            {
                Create.Index(indexName).OnTable(DatabaseTableName)
                    .OnColumn(WebsiteIdColumnName)
                    .Ascending()
                    .WithOptions()
                    .NonClustered()
                    .Do();
            }
        }

        private void AlterUserDefinedTableType(IUmbracoDatabase db)
        {
            db.Execute($@"
                IF TYPE_ID('{TableTypeName}') IS NOT NULL
                    DROP TYPE {TableTypeName};

                    CREATE TYPE {TableTypeName} AS TABLE(
	                    [PageId] [uniqueidentifier] NOT NULL,
                        [WebsiteId] [int] NOT NULL,
	                    [Culture] [char](8) NOT NULL,
	                    [ContentBlockDefinitionId] [uniqueidentifier] NOT NULL,
	                    [Amount] [int] NOT NULL
                    )
            ");
        }
    }
}
