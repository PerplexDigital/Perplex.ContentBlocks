using System.ComponentModel.DataAnnotations;
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

    public override IEnumerable<ValidationResult> Validate(IDictionary<string, object> configuration)
    {
        return base.Validate(configuration);
    }

    public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
    {
        // TODO: Umbraco never calls this method. When the value editor is loaded they also call "ToConfigurationEditor" ....

        TransformStructure(configuration);
        return base.ToValueEditor(configuration);

        // Structure will be sent to the UI as { "blocks": true/false, "header": true/false } rather than "Blocks" / "Header" / "All"
        static void TransformStructure(IDictionary<string, object> config)
        {
            if (!config.TryGetValue(StructureKey, out var value) ||
                !Enum.TryParse(value.ToString(), out Structure structure))
            {
                return;
            }

            config[StructureKey] = new
            {
                blocks = structure.HasFlag(Structure.Blocks),
                header = structure.HasFlag(Structure.Header)
            };
        }
    }

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

            if (!source.TryGetValue(LegacyVersionKey, out var versionObj) ||
                !int.TryParse(versionObj?.ToString(), out var version) ||
                version >= 4)
            {
                // Up to date
                return source;
            }

            source.Remove(LegacyVersionKey);
            source[VersionKey] = ContentBlocksConfiguration.CurrentVersion;

            switch (version)
            {
                case 1:
                    // HidePropertyGroupContainer will be read as "false" when this option
                    // did not exist before, whereas our default is "true" at the moment.
                    // To not suddenly change existing editors upon update we should set
                    // this setting to "true" for existing editors.
                    source[HidePropertyGroupContainerKey] = true;
                    goto default;

                default:
                    return MigrateProperties(source);
            }

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
}
