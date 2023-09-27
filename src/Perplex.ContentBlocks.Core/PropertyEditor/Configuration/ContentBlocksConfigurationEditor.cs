using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using static Perplex.ContentBlocks.Constants.PropertyEditor.Configuration;

namespace Perplex.ContentBlocks.PropertyEditor.Configuration;

public class ContentBlocksConfigurationEditor : ConfigurationEditor<ContentBlocksConfiguration>
{
    public ContentBlocksConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser)
        : base(ioHelper, editorConfigurationParser)
    {
        Fields.AddRange(new[]
        {
            new ConfigurationField
            {
                Name = "Structure",
                Description = "Sets the structure",
                Key = StructureKey,
                View = StructureViewName,
            },

            new ConfigurationField
            {
                Name = "Disable preview",
                Description = "Disables the preview feature",
                Key = DisablePreviewKey,
                View = DisablePreviewViewName,
            },

            new ConfigurationField
            {
                Name = "Hide label",
                Description = "Hides the data type label",
                Key = HideLabelKey,
                View = HideLabelViewName,
            },

            new ConfigurationField
            {
                Name = "Hide property group container",
                Description = "Hides the property group container that holds this editor",
                Key = HidePropertyGroupContainerKey,
                View = HidePropertyGroupContainerViewName,
            },

            new ConfigurationField
            {
                Name = "Allow adding blocks without header",
                Description = "Blocks can be added without having to set a header first",
                Key = AllowBlocksWithoutHeaderKey,
                View = AllowBlocksWithoutHeaderViewName,
            },
        });
    }

    public override object DefaultConfigurationObject { get; }
        = ContentBlocksConfiguration.DefaultConfiguration;

    public override IDictionary<string, object> DefaultConfiguration
        => ToConfigurationEditor(DefaultConfigurationObject);

    public override ContentBlocksConfiguration FromConfigurationEditor(IDictionary<string, object?>? editorValues, ContentBlocksConfiguration? configuration)
    {
        var hideLabel = GetBool(editorValues, HideLabelKey);
        var structure = GetStructure(editorValues);
        var disablePreview = GetBool(editorValues, DisablePreviewKey);
        var hidePropertyGroupContainer = GetBool(editorValues, HidePropertyGroupContainerKey);
        var requireHeaderForBlocks = GetBool(editorValues, AllowBlocksWithoutHeaderKey);

        var defaultConfig = ContentBlocksConfiguration.DefaultConfiguration;

        return new ContentBlocksConfiguration
        {
            // When saved we store the current version,
            // Any transformations should have been applied before
            // and the user has now actively saved the configuration again.
            Version = ContentBlocksConfiguration.VERSION,

            Structure = structure ?? defaultConfig.Structure,
            DisablePreview = disablePreview ?? defaultConfig.DisablePreview,
            HideLabel = hideLabel ?? defaultConfig.HideLabel,
            HidePropertyGroupContainer = hidePropertyGroupContainer ?? defaultConfig.HidePropertyGroupContainer,
            AllowBlocksWithoutHeader = requireHeaderForBlocks ?? defaultConfig.AllowBlocksWithoutHeader,
        };
    }

    public override Dictionary<string, object> ToConfigurationEditor(ContentBlocksConfiguration? configuration)
    {
        if (configuration is null)
        {
            return new Dictionary<string, object>();
        }

        return new Dictionary<string, object>
        {
            [VersionKey] = configuration.Version,

            [StructureKey] = new
            {
                blocks = configuration.Structure.HasFlag(Structure.Blocks),
                header = configuration.Structure.HasFlag(Structure.Header),
            },

            [DisablePreviewKey] = configuration.DisablePreview,

            [HideLabelKey] = configuration.HideLabel,

            [HidePropertyGroupContainerKey] = configuration.HidePropertyGroupContainer,

            [AllowBlocksWithoutHeaderKey] = configuration.AllowBlocksWithoutHeader,
        };
    }

    public override object FromDatabase(string? configuration, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
    {
        if (configuration == null || configuration.Trim() == "{}")
        {
            // Special case: empty configuration object.
            // That is not allowed, return the default.
            return ContentBlocksConfiguration.DefaultConfiguration;
        }

        try
        {
            if (base.FromDatabase(configuration, configurationEditorJsonSerializer) is ContentBlocksConfiguration contentBlocksConfiguration)
            {
                return ApplyMigrations(contentBlocksConfiguration);
            }
            else
            {
                return ContentBlocksConfiguration.DefaultConfiguration;
            };
        }
        catch
        {
            return ContentBlocksConfiguration.DefaultConfiguration;
        }
    }

    private static ContentBlocksConfiguration ApplyMigrations(ContentBlocksConfiguration source)
    {
        if (source.Version == ContentBlocksConfiguration.VERSION)
        {
            // Already the latest version.
            return source;
        }

        switch (source.Version)
        {
            case 1:
                // HidePropertyGroupContainer will be read as "false" when this option
                // did not exist before, whereas our default is "true" at the moment.
                // To not suddenly change existing editors upon update we should set
                // this setting to "true" for existing editors.
                source.HidePropertyGroupContainer = true;
                break;

            default:
                break;
        }

        return source;
    }

    private static bool? GetBool(IDictionary<string, object?>? config, string key)
    {
        if (config != null && config.TryGetValue(key, out object? value) &&
            ParseBoolean(value?.ToString()) is bool boolValue)
        {
            return boolValue;
        };

        return null;
    }

    private static bool? ParseBoolean(string? input)
    {
        if (bool.TryParse(input, out bool value))
            return value;

        // Parse manually
        if (input == "0")
            return false;

        if (input == "1")
            return true;

        return null;
    }

    private static Structure? GetStructure(IDictionary<string, object?>? config)
    {
        Structure structure = Structure.None;

        if (config != null &&
            config.TryGetValue(StructureKey, out object? obj) &&
            obj is JObject structureObj)
        {
            if (structureObj.Value<bool>("blocks"))
                structure |= Structure.Blocks;

            if (structureObj.Value<bool>("header"))
                structure |= Structure.Header;

            return structure;
        };

        return null;
    }
}
