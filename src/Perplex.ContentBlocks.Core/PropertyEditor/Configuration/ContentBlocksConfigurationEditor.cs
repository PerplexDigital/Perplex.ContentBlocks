using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;
using static Perplex.ContentBlocks.Constants.PropertyEditor.Configuration;

namespace Perplex.ContentBlocks.PropertyEditor.Configuration;

public class ContentBlocksConfigurationEditor(IIOHelper ioHelper, IConfigurationEditorJsonSerializer configEditorSerializer)
    : ConfigurationEditor<ContentBlocksConfiguration>(ioHelper)
{
    public override IDictionary<string, object> DefaultConfiguration
        => ToDictionary(ContentBlocksConfiguration.DefaultConfiguration);

    private Dictionary<string, object> ToDictionary(ContentBlocksConfiguration configuration)
    {
        var json = configEditorSerializer.Serialize(configuration);
        return configEditorSerializer.TryDeserialize<Dictionary<string, object>>(json, out var config)
            ? config
            : [];
    }

    public override IDictionary<string, object> FromDatabase(string? configuration, IConfigurationEditorJsonSerializer serializer)
    {
        var config = base.FromDatabase(configuration, serializer);
        return ApplyMigrations(config);

        static IDictionary<string, object> ApplyMigrations(IDictionary<string, object> source)
        {
            const string LegacyVersionKey = "Version";

            if (source.TryGetValue(LegacyVersionKey, out var legacyVersion))
            {
                source[VersionKey] = legacyVersion;
            }

            if (source.TryGetValue(VersionKey, out var versionObj) &&
                int.TryParse(versionObj?.ToString(), out int version) &&
                version >= ContentBlocksConfiguration.CurrentVersion)
            {
                // Already up-to-date
                return source;
            }

            source.Remove(LegacyVersionKey);
            source[VersionKey] = ContentBlocksConfiguration.CurrentVersion;

            return MigrateProperties(source);

            static IDictionary<string, object> MigrateProperties(IDictionary<string, object> dict)
            {
                return dict
                    .Select(kv => (key: MigrateKey(kv.Key), value: MigrateValue(kv)))
                    .ToDictionary(t => t.key, t => t.value);
            }

            static string MigrateKey(string key) =>
                // Configuration from ContentBlocks v1 - v3 was PascalCase, convert to current camelCase format.
                key.ToFirstLowerInvariant();

            static object MigrateValue(KeyValuePair<string, object> kv)
            {
                const string LegacyStructureKey = "Structure";

                return kv.Key switch
                {
                    LegacyStructureKey => kv.Value switch
                    {
                        // Some versions stored the Structure as an int, convert to string
                        int structure => ((Structure)structure).ToString(),
                        _ => kv.Value,
                    },
                    _ => kv.Value,
                };
            }
        }
    }

    public override string ToDatabase(IDictionary<string, object> configuration, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
    {
        if (!configuration.TryGetValue(VersionKey, out var versionObj) ||
            !int.TryParse(versionObj?.ToString(), out var version) ||
            version < ContentBlocksConfiguration.CurrentVersion)
        {
            configuration[VersionKey] = ContentBlocksConfiguration.CurrentVersion;
        }

        return base.ToDatabase(configuration, configurationEditorJsonSerializer);
    }
}
