using Athlon.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Umbraco.Core.Scoping;
using static Athlon.Constants.ContentBlocks.Usage;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlocksUsageDatabaseRepository : IContentBlockUsageRepository
    {
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private readonly IScopeProvider _scopeProvider;

        public ContentBlocksUsageDatabaseRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        private void InScope(Action<IScope> action, bool completeAfter = true)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                action(scope);
                if (completeAfter)
                {
                    scope.Complete();
                }
            }
        }

        private T InScope<T>(Func<IScope, T> func, bool completeAfter = true)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                T result = func(scope);

                if (completeAfter)
                {
                    scope.Complete();
                }

                return result;
            }
        }

        public void Clear()
        {
            InScope(scope =>
                scope.Database.Execute($"TRUNCATE TABLE {DatabaseTableName}"));
        }

        public void Clear(Guid contentBlockDefinitionId, Guid pageId, string culture)
        {
            InScope(scope =>
            {
                string query = $@"
                    DELETE FROM {DatabaseTableName}
                    WHERE {nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)} = @0
                    AND {nameof(ContentBlocksUsageDatabaseRow.PageId)} = @1
                    AND {nameof(ContentBlocksUsageDatabaseRow.Culture)} = @2";

                scope.Database.Execute(query, contentBlockDefinitionId, pageId, culture);
            });
        }

        public void ClearDefinition(Guid contentBlockDefinitionId, string culture = null)
        {
            InScope(scope =>
            {
                string query = $@"
                    DELETE FROM {DatabaseTableName}
                    WHERE {nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)} = @0
                    AND (@1 IS NULL OR {nameof(ContentBlocksUsageDatabaseRow.Culture)} = @1)";

                scope.Database.Execute(query, contentBlockDefinitionId, culture);
            });
        }

        public void ClearPage(Guid pageId, string culture = null)
        {
            InScope(scope =>
            {
                string query = $@"
                    DELETE FROM {DatabaseTableName}
                    WHERE {nameof(ContentBlocksUsageDatabaseRow.PageId)} = @0
                    AND (@1 IS NULL OR {nameof(ContentBlocksUsageDatabaseRow.Culture)} = @1)";

                scope.Database.Execute(query, pageId, culture);
            });
        }

        public IEnumerable<IContentBlockUsage> GetAllUses(int? websiteId = null, string culture = null)
        {
            IEnumerable<ContentBlocksUsageDatabaseRow> rows = InScope<IEnumerable<ContentBlocksUsageDatabaseRow>>(scope =>
            {
                string query = $@"
                    SELECT * FROM {DatabaseTableName}
                    WHERE   (@0 IS NULL OR {nameof(ContentBlocksUsageDatabaseRow.Culture)} = @0) AND
                            (@1 IS NULL OR {nameof(ContentBlocksUsageDatabaseRow.WebsiteId)} = @1)";
                return scope.Database.Fetch<ContentBlocksUsageDatabaseRow>(query, culture, websiteId);
            });

            return rows
                .GroupBy(r => r.ContentBlockDefinitionId)
                .Select(g => CreateUsage(g.Key, g));
        }

        public IContentBlockUsage GetUsage(Guid contentBlockDefinitionId, int? websiteId = null, string culture = null)
        {
            IEnumerable<ContentBlocksUsageDatabaseRow> rows = InScope<IEnumerable<ContentBlocksUsageDatabaseRow>>(scope =>
            {
                string query = $@"
                    SELECT * FROM {DatabaseTableName}
                    WHERE {nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)} = @0
                    AND (@1 IS NULL OR {nameof(ContentBlocksUsageDatabaseRow.Culture)} = @1)
                    AND (@2 IS NULL OR {nameof(ContentBlocksUsageDatabaseRow.WebsiteId)} = @2)";

                return scope.Database.Fetch<ContentBlocksUsageDatabaseRow>(query, contentBlockDefinitionId, culture, websiteId);
            });

            return CreateUsage(contentBlockDefinitionId, rows);
        }

        private IContentBlockUsage CreateUsage(Guid contentBlockDefinitionId, IEnumerable<ContentBlocksUsageDatabaseRow> rows)
        {
            return new ContentBlockUsage
            {
                ContentBlockDefinitionId = contentBlockDefinitionId,
                PageUses = rows.GroupBy(r => new { r.PageId, r.WebsiteId, r.Culture }).Select(g => CreatePageUsage(contentBlockDefinitionId, g.Key.PageId, g.Key.WebsiteId, g.Key.Culture, g))
            };
        }

        private IContentBlockPageUsage CreatePageUsage(Guid contentBlockDefinitionId, Guid pageId, int websiteId, string culture, IEnumerable<ContentBlocksUsageDatabaseRow> rows)
        {
            return new ContentBlockPageUsage
            {
                ContentBlockDefinitionId = contentBlockDefinitionId,
                PageId = pageId,
                Culture = culture,
                UsageAmount = rows.Sum(r => r.Amount)
            };
        }

        private IEnumerable<ContentBlocksUsageDatabaseRow> CreateDatabaseRows(IContentBlockUsage usage)
        {
            return usage.PageUses.Select(pu => new ContentBlocksUsageDatabaseRow
            {
                ContentBlockDefinitionId = usage.ContentBlockDefinitionId,
                PageId = pu.PageId,
                Culture = pu.Culture,
                WebsiteId = pu.WebsiteId,
                Amount = pu.UsageAmount
            });
        }

        public void Save(IContentBlockUsage usage)
        {
            var rows = CreateDatabaseRows(usage);
            SaveRows(rows);
        }

        public void Save(IEnumerable<IContentBlockUsage> uses)
        {
            var rows = uses.SelectMany(CreateDatabaseRows);
            SaveRows(rows);
        }

        private void SaveRows(IEnumerable<ContentBlocksUsageDatabaseRow> rows)
        {
            if (rows?.Any() != true)
            {
                return;
            }

            var dtRows = rows.ToDataTable();
            SqlParameter param = new SqlParameter("rows", dtRows)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = TableTypeName
            };

            InScope(scope =>
            {
                scope.Database.Execute($@"
                    INSERT INTO {DatabaseTableName}(
                        {nameof(ContentBlocksUsageDatabaseRow.PageId)},
                        {nameof(ContentBlocksUsageDatabaseRow.WebsiteId)},
                        {nameof(ContentBlocksUsageDatabaseRow.Culture)},
                        {nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)},
                        {nameof(ContentBlocksUsageDatabaseRow.Amount)}
                    ) SELECT
                        {nameof(ContentBlocksUsageDatabaseRow.PageId)},
                        {nameof(ContentBlocksUsageDatabaseRow.WebsiteId)},
                        {nameof(ContentBlocksUsageDatabaseRow.Culture)},
                        {nameof(ContentBlocksUsageDatabaseRow.ContentBlockDefinitionId)},
                        {nameof(ContentBlocksUsageDatabaseRow.Amount)}
                    FROM @0",
                    param);
            });
        }

        public void WithLock(Action action)
        {
            _lock.Wait();

            try
            {
                action();
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
