using Microsoft.Extensions.Logging;
using NPoco;
using Perplex.ContentBlocks.Utils;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using static Umbraco.Cms.Core.Constants.PropertyEditors;

namespace Perplex.ContentBlocks.Migrations.M001_MigrateFromNestedContentToBlockList;

public class MigrateFromNestedContentToBlockList
(
    IMigrationContext context,
    IContentTypeService contentTypeService,
    IEnumerable<IContentBlocksPropertyValueMigrator> propertyValueMigrators,
    ILogger<MigrateFromNestedContentToBlockList> logger
) : AsyncMigrationBase(context)
{
    private const string _blockListUiEditorAlias = "Umb.PropertyEditorUi.BlockList";
    private const int _numberOfContentVersionsToMigrate = 8;

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };
    private readonly Dictionary<string, IContentType> _cachedContentTypeByAlias = new(StringComparer.OrdinalIgnoreCase);

    protected override Task MigrateAsync()
    {
        MigrateContentBlocksPropertyData();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Migrates the property data of Perplex.ContentBlocks from Nested Content to Block List format.
    /// This also migrates the Nested Content data types used within Perplex.ContentBlocks to use the Block List property editor.
    /// </summary>
    private void MigrateContentBlocksPropertyData()
    {
        var sw = Stopwatch.StartNew();

        logger.LogInformation("[Perplex.ContentBlocks] Reading property data from database ...");
        var propertyData = GetPropertyData();
        logger.LogInformation("[Perplex.ContentBlocks] Property data rows found: {count}", propertyData.Length);

        var nestedContentDataTypes = GetDataTypes("Umbraco.NestedContent").ToDictionary(dt => dt.NodeId);

        var migratedDataTypes = new Dictionary<int, UmbracoDataType>();

        logger.LogInformation("[Perplex.ContentBlocks] Migrating property data ...");
        var migratedPropertyData = MigratePropertyData(propertyData);

        logger.LogInformation("[Perplex.ContentBlocks] Property data rows migrated: {count}", migratedPropertyData.Length);
        logger.LogInformation("[Perplex.ContentBlocks] Nested Content data types migrated: {count}", migratedDataTypes.Values.Count);

        if (migratedPropertyData.Length > 0)
        {
            logger.LogInformation("[Perplex.ContentBlocks] Writing migrated property data to database ...");
            PersistPropertyData(migratedPropertyData);
        }

        if (migratedDataTypes.Values.Count > 0)
        {
            logger.LogInformation("[Perplex.ContentBlocks] Writing migrated data types to database ...");
            PersistDataTypes(migratedDataTypes.Values);
        }

        sw.Stop();

        var elapsed = GetHumanReadableElapsed(sw.Elapsed);
        logger.LogInformation("[Perplex.ContentBlocks] Migration completed in {elapsed}", elapsed);

        UmbracoPropertyData[] GetPropertyData()
        {
            // Note: due to an NPoco bug the query MUST START with ";" as the first character.
            // Do not add whitespace before it then it will break again because NPoco will then
            // insert a SELECT statement before the CTE.
            var query = @";WITH RankedVersions AS (
                    SELECT pd.*, ROW_NUMBER() OVER (
                        PARTITION BY v.nodeId, pd.propertyTypeId, pd.languageId, pd.segment
                        ORDER BY pd.versionId DESC
                    ) versionRank
                    FROM umbracoPropertyData pd
                    JOIN umbracoContentVersion v
                    ON pd.versionId = v.id
                    JOIN cmsPropertyType pt
                    ON pd.propertyTypeId = pt.id
                    JOIN umbracoDataType dt
                    ON pt.dataTypeId = dt.nodeId
                    WHERE dt.propertyEditorAlias = @propertyEditorAlias
                )

                SELECT id, versionId, propertyTypeId, languageId, segment, intValue, decimalValue, dateValue, varcharValue, textValue
                FROM RankedVersions
                WHERE VersionRank <= @versions";

            var args = new
            {
                propertyEditorAlias = Constants.PropertyEditor.Alias,
                versions = _numberOfContentVersionsToMigrate,
            };

            return [.. Database.Fetch<UmbracoPropertyData>(query, args)];
        }

        UmbracoPropertyData[] MigratePropertyData(UmbracoPropertyData[] propertyDatas)
        {
            if (propertyDatas.Length == 0)
            {
                return [];
            }

            var migrated = new List<UmbracoPropertyData>();

            foreach (var propertyData in propertyDatas)
            {
                if (!JsonUtils.TryParseJson(propertyData.TextValue, out var node))
                {
                    continue;
                }

                if (!MigrateContentBlocks(node))
                {
                    continue;
                }

                propertyData.TextValue = node.ToJsonString(_jsonOptions);

                migrated.Add(propertyData);
            }

            return [.. migrated];
        }

        bool MigrateContentBlocks(JsonNode node)
        {
            // We need to migrate ContentBlocks data of versions 1-3 but not newer.
            const int MaxDataVersionToMigrate = 3;
            const int CurrentDataVersion = 4;

            if (node["version"]?.GetValue<int>() > MaxDataVersionToMigrate)
            {
                // Already migrated to the new(er) format.
                return false;
            }

            node["version"] = CurrentDataVersion;

            if (node["header"] is JsonObject header)
            {
                MigrateContentBlock(header);
            }

            if (node["blocks"] is JsonArray blocks)
            {
                foreach (var block in blocks.OfType<JsonObject>())
                {
                    MigrateContentBlock(block);
                }
            }

            return true;
        }

        void MigrateContentBlock(JsonObject block)
        {
            // TODO: Consider what to do with the "variants" property.
            // We no longer support variants and if we were to add support we should use Umbraco's built-in
            // Block Level Variants feature which stores the variant data in the block's content instead of in a separate property.
            // For now we will just ignore it; i.e. the variants data will stay in its old format in the block.

            if (block["content"] is not JsonArray contentArray ||
                contentArray.Count <= 0 ||
                contentArray[0] is not JsonObject content)
            {
                // No existing valid content, initialize with an empty object
                block["content"] = new JsonObject();
                return;
            }

            if (content["ncContentTypeAlias"]?.ToString() is not string contentTypeAlias ||
                GetContentType(contentTypeAlias) is not IContentType contentType)
            {
                // Cannot determine content type used by this block, ignore.
                return;
            }

            var values = MigrateValues(content, contentType);

            block["content"] = new JsonObject
            {
                ["contentTypeKey"] = contentType.Key,
                ["key"] = content["key"]?.DeepClone(),
                ["values"] = values
            };
        }

        JsonArray MigrateValues(JsonObject content, IContentType contentType)
        {
            var values = new JsonArray();

            foreach (var property in contentType.CompositionPropertyTypes)
            {
                if (!content.ContainsKey(property.Alias))
                {
                    continue;
                }

                if (property.PropertyEditorAlias == "Umbraco.NestedContent" &&
                    !migratedDataTypes.ContainsKey(property.DataTypeId) &&
                    nestedContentDataTypes.TryGetValue(property.DataTypeId, out var ncDataType) &&
                    MigrateNestedContentDataType(ncDataType) is UmbracoDataType migratedDataType)
                {
                    // Migrate the Nested Content data type to Block List format
                    migratedDataTypes[property.DataTypeId] = migratedDataType;
                }

                var oldValue = content[property.Alias];
                var newValue = MigrateValue(oldValue, property.PropertyEditorAlias);

                foreach (var migrator in propertyValueMigrators)
                {
                    if (migrator.MigratePropertyValue(oldValue, property, contentType, out var migratedValue))
                    {
                        newValue = migratedValue;
                    }
                }

                var value = new JsonObject
                {
                    ["editorAlias"] = property.PropertyEditorAlias,
                    ["culture"] = null,
                    ["segment"] = null,
                    ["alias"] = property.Alias,
                    ["value"] = newValue,
                };

                values.Add(value);
            }

            return values;
        }

        JsonNode? MigrateValue(JsonNode? node, string propertyEditorAlias)
        {
            if (propertyEditorAlias != "Umbraco.NestedContent")
            {
                // Return the original value
                return node?.DeepClone();
            }

            if (!JsonUtils.TryParseJson(node?.ToString(), out JsonNode? value))
            {
                // In NestedContent everything is stored as a string even if it's actually an Array or Object
                // so we always first try to parse it as a complex object and use that if possible.
                // BlockList stores the value as a proper complex object so this is necessary to make
                // many values that we do not migrate work correctly.
                // For example "[]" should be a real JsonArray and not a string.
                value = node;
            }

            return MigrateNestedContentPropertyValue(value);
        }

        JsonNode? MigrateNestedContentPropertyValue(JsonNode? node)
        {
            if (node is not JsonArray ncArray ||
                ncArray.Count == 0)
            {
                // Cannot determine content type used by this block, ignore.
                return node?.DeepClone();
            }

            var layouts = new JsonArray();
            var contentDatas = new JsonArray();
            var exposes = new JsonArray();

            foreach (var ncContent in ncArray.OfType<JsonObject>())
            {
                if (ncContent["ncContentTypeAlias"]?.ToString() is not string contentTypeAlias ||
                    GetContentType(contentTypeAlias) is not IContentType contentType)
                {
                    // Cannot determine content type used by this block, ignore.
                    continue;
                }

                var values = MigrateValues(ncContent, contentType);

                var key = ncContent["key"]?.ToString();

                layouts.Add(new JsonObject
                {
                    ["contentKey"] = key,
                });

                contentDatas.Add(new JsonObject
                {
                    ["key"] = key,
                    ["contentTypeKey"] = contentType.Key,
                    ["values"] = values
                });

                exposes.Add(new JsonObject
                {
                    ["contentKey"] = key,
                    ["culture"] = null,
                    ["segment"] = null
                });
            }

            // BlockList format
            return new JsonObject
            {
                ["contentData"] = contentDatas,

                ["settingsData"] = new JsonArray(),

                ["expose"] = exposes,

                ["Layout"] = new JsonObject
                {
                    ["Umbraco.BlockList"] = layouts,
                },
            };
        }

        void PersistPropertyData(IEnumerable<UmbracoPropertyData> propertyDatas)
        {
            if (DatabaseType == DatabaseType.SQLite)
            {
                // SQLite does not support true bulk insert and the UPDATE .. JOIN syntax so
                // we will use the simple but slow method there.
                ForEachUpdate();
                return;
            }

            // Non SQLite database types should be SQL Server which can do bulk insert + update
            BulkUpdate();

            void ForEachUpdate()
            {
                foreach (var propertyData in propertyDatas)
                {
                    var query = @"
                        UPDATE umbracoPropertyData
                        SET textValue = @textValue
                        WHERE id = @id";

                    var args = new
                    {
                        textValue = propertyData.TextValue,
                        id = propertyData.Id
                    };

                    Database.Execute(query, args);
                }
            }

            void BulkUpdate()
            {
                Database.BeginTransaction();

                Database.Execute(@"
                    CREATE TABLE #ContentBlocksMigration (
                        id INT NOT NULL,
                        textValue NVARCHAR(MAX) NULL)");

                var migratedValues = propertyDatas.Select(pd => new MigratedPropertyValue
                {
                    Id = pd.Id,
                    TextValue = pd.TextValue,
                }).ToArray();

                Database.InsertBulk(migratedValues);

                Database.OneTimeCommandTimeout = 600; // 10 minutes

                Database.Execute(@"
                    UPDATE target
                    SET target.textValue = src.textValue
                    FROM umbracoPropertyData target
                    INNER JOIN #ContentBlocksMigration src ON target.id = src.id");

                Database.Execute("DROP TABLE #ContentBlocksMigration");

                Database.CompleteTransaction();
            }
        }

        UmbracoDataType? MigrateNestedContentDataType(UmbracoDataType dataType)
        {
            var config = MapNestedContentConfig(dataType.Config);

            return new UmbracoDataType
            {
                NodeId = dataType.NodeId,
                PropertyEditorAlias = Aliases.BlockList,
                PropertyEditorUiAlias = _blockListUiEditorAlias,
                DbType = dataType.DbType,
                Config = config,
            };
        }

        string? MapNestedContentConfig(string? configStr)
        {
            if (!JsonUtils.TryParseJson(configStr, out var config))
            {
                // Cannot parse, just return the original config
                return configStr;
            }

            var blockListConfig = new JsonObject
            {
                // NestedContent only has inline editing so let's enable that for the Block List version as well.
                ["useInlineEditingAsDefault"] = true
            };

            var min = config["minItems"]?.GetValue<int?>();
            var max = config["maxItems"]?.GetValue<int?>();

            if (min > 0 || max > 0)
            {
                var validationLimit = blockListConfig["validationLimit"] = new JsonObject();

                if (min > 0)
                {
                    validationLimit["min"] = min.Value;
                }

                if (max > 0)
                {
                    validationLimit["max"] = max.Value;
                }
            }

            var blocks = new JsonArray();

            if (config["contentTypes"] is JsonArray ncContentTypes &&
                ncContentTypes.Count > 0)
            {
                foreach (var ncContentType in ncContentTypes.OfType<JsonObject>())
                {
                    if (ncContentType["ncAlias"]?.ToString() is not string contentTypeAlias ||
                        GetContentType(contentTypeAlias) is not IContentType contentType)
                    {
                        continue;
                    }

                    blocks.Add(new JsonObject
                    {
                        ["contentElementTypeKey"] = contentType.Key
                    });
                }

                if (blocks.Count > 0)
                {
                    blockListConfig["blocks"] = blocks;
                }
            }

            if (min == 1 && max == 1 && blocks.Count == 1)
            {
                blockListConfig["useSingleBlockMode"] = true;
            }

            return blockListConfig.ToJsonString(_jsonOptions);
        }

        void PersistDataTypes(IEnumerable<UmbracoDataType> dataTypes)
        {
            foreach (var dataType in dataTypes)
            {
                var query = @"
                    UPDATE umbracoDataType
                    SET propertyEditorAlias = @propertyEditorAlias,
                        config = @config,
                        dbType = @dbType,
                        propertyEditorUiAlias = @propertyEditorUiAlias
                    WHERE nodeId = @nodeId";

                var args = new
                {
                    nodeId = dataType.NodeId,
                    dbType = dataType.DbType,
                    propertyEditorAlias = dataType.PropertyEditorAlias,
                    propertyEditorUiAlias = dataType.PropertyEditorUiAlias,
                    config = dataType.Config,
                };

                Database.Execute(query, args);
            }
        }
    }

    private IContentType? GetContentType(string contentTypeAlias)
    {
        if (string.IsNullOrWhiteSpace(contentTypeAlias))
        {
            return null;
        }

        if (!_cachedContentTypeByAlias.TryGetValue(contentTypeAlias, out var cachedContentType))
        {
            var contentType = contentTypeService.Get(contentTypeAlias);
            if (contentType is null)
            {
                return null;
            }

            _cachedContentTypeByAlias[contentTypeAlias] = cachedContentType = contentType;
        }

        return cachedContentType;
    }

    private UmbracoDataType[] GetDataTypes(string propertyEditorAlias)
    {
        var query = @"
            SELECT *
            FROM umbracoDataType
            WHERE propertyEditorAlias = @propertyEditorAlias";

        return [.. Database.Fetch<UmbracoDataType>(query, new { propertyEditorAlias })];
    }

    private static string GetHumanReadableElapsed(TimeSpan ts)
    {
        int amount;
        string unit;

        if (ts.TotalSeconds < 10)
        {
            amount = (int)Math.Round(ts.TotalMilliseconds);
            if (amount == 0) amount = 1; // handle very small times
            unit = amount == 1 ? "millisecond" : "milliseconds";
        }

        else if (ts.TotalSeconds < 600)
        {
            amount = (int)Math.Round(ts.TotalSeconds);
            unit = amount == 1 ? "second" : "seconds";
        }

        else if (ts.TotalMinutes < 600)
        {
            amount = (int)Math.Round(ts.TotalMinutes);
            unit = amount == 1 ? "minute" : "minutes";
        }

        else
        {
            amount = (int)Math.Round(ts.TotalHours);
            unit = amount == 1 ? "hour" : "hours";
        }

        return $"{amount} {unit}";
    }

    // Copied from Umbraco.Cms.Infrastructure.Persistence.Dtos.PropertyDataDto and dropped the PropertyTypeDto + Value properties
    internal class UmbracoPropertyData
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public int PropertyTypeId { get; set; }
        public int? LanguageId { get; set; }
        public string? Segment { get; set; }
        public int? IntValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string? VarcharValue { get; set; }
        public string? TextValue { get; set; }
    }

    // Copied from Umbraco.Cms.Infrastructure.Persistence.Dtos and dropped the NodeDto property
    internal class UmbracoDataType
    {
        public int NodeId { get; set; }
        public string PropertyEditorAlias { get; set; } = null!;
        public string? PropertyEditorUiAlias { get; set; }
        public string DbType { get; set; } = null!;
        public string? Config { get; set; }
    }

    [TableName("#ContentBlocksMigration")]
    [PrimaryKey(nameof(Id), AutoIncrement = false)]
    internal class MigratedPropertyValue
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("textValue")]
        public string? TextValue { get; set; }
    }
}
