using Athlon.Usage;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;
using static Athlon.Constants.ContentBlocks.Usage;

namespace Perplex.ContentBlocks.Features.Migrations
{
    public class ContentBlocksMigrationContentBlockUsageDatabaseSetup : MigrationBase
    {
        private readonly IScopeProvider _scopeProvider;

        public ContentBlocksMigrationContentBlockUsageDatabaseSetup(IMigrationContext context, IScopeProvider scopeProvider)
            : base(context)
        {
            _scopeProvider = scopeProvider;
        }

        public override void Migrate()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                CreateDatabase();
                CreateIndexes();
                CreateTableType(scope.Database);

                scope.Complete();
            }
        }

        private void CreateDatabase()
        {
            if (!TableExists(DatabaseTableName))
            {
                Create.Table(DatabaseTableName)
                    .WithColumn(nameof(ContentBlocksUsageDatabaseRow.PageId)).AsGuid().NotNullable()
                    .WithColumn(nameof(ContentBlocksUsageDatabaseRow.Culture)).AsString().NotNullable()
                    .WithColumn(nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)).AsGuid().NotNullable()
                    .WithColumn(nameof(ContentBlocksUsageDatabaseRow.Amount)).AsInt32().NotNullable()
                    .Do();
            }
        }

        private void CreateIndexes()
        {
            CreateIndex(nameof(ContentBlocksUsageDatabaseRow.PageId));
            CreateIndex(nameof(ContentBlocksUsageDatabaseRow.Culture));
            CreateIndex(nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId));
        }

        private void CreateIndex(string columnName)
        {
            string indexName = $"IX_{DatabaseTableName}_{columnName}";

            if (!IndexExists(indexName))
            {
                Create.Index(indexName).OnTable(DatabaseTableName)
                .OnColumn(columnName)
                .Ascending()
                .WithOptions().NonClustered()
                .Do();
            }
        }

        private void CreateTableType(IUmbracoDatabase db)
        {
            db.Execute($@"
                IF TYPE_ID('{TableTypeName}') IS NULL
                    CREATE TYPE {TableTypeName} AS TABLE(
	                    [{nameof(ContentBlocksUsageDatabaseRow.PageId)}] [uniqueidentifier] NOT NULL,
	                    [{nameof(ContentBlocksUsageDatabaseRow.Culture)}] [char](8) NOT NULL,
	                    [{nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)}] [uniqueidentifier] NOT NULL,
	                    [{nameof(ContentBlocksUsageDatabaseRow.Amount)}] [int] NOT NULL
                    )
            ");
        }
    }
}
