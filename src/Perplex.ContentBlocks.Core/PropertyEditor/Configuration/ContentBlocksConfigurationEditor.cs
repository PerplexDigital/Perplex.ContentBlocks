using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using static Perplex.ContentBlocks.Constants.PropertyEditor.Configuration;

#if NET5_0_OR_GREATER
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
#elif NET472
using Umbraco.Core.PropertyEditors;
#endif

namespace Perplex.ContentBlocks.PropertyEditor.Configuration
{
    public class ContentBlocksConfigurationEditor : ConfigurationEditor<ContentBlocksConfiguration>
    {
#if NET5_0_OR_GREATER

        public ContentBlocksConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
#elif NET472
        public ContentBlocksConfigurationEditor()
#endif
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

        /// <summary>
        /// Current configuration version.
        /// </summary>
        public const int Version = 3;

        internal static readonly ContentBlocksConfiguration _defaultConfiguration = new ContentBlocksConfiguration
        {
            Version = Version,

            HideLabel = true,
            Structure = Structure.Blocks | Structure.Header,
            DisablePreview = false,

            // It is quite likely this will default to "false" in the future
            // considering hiding the property group container is messing with
            // the default Umbraco UI and also causes some flickering upon page load
            // when the group is being hidden after our editor is initialized.
            HidePropertyGroupContainer = true,

            AllowBlocksWithoutHeader = false,
        };

        public override object DefaultConfigurationObject { get; }
            = _defaultConfiguration;

        public override IDictionary<string, object> DefaultConfiguration
            => ToConfigurationEditor(_defaultConfiguration);

        public override ContentBlocksConfiguration FromConfigurationEditor(IDictionary<string, object> editorValues, ContentBlocksConfiguration configuration)
        {
            var hideLabel = GetBool(editorValues, HideLabelKey);
            var structure = GetStructure(editorValues);
            var disablePreview = GetBool(editorValues, DisablePreviewKey);
            var hidePropertyGroupContainer = GetBool(editorValues, HidePropertyGroupContainerKey);
            var requireHeaderForBlocks = GetBool(editorValues, AllowBlocksWithoutHeaderKey);

            return new ContentBlocksConfiguration
            {
                // When saved we store the current version,
                // Any transformations should have been applied before
                // and the user has now actively saved the configuration again.
                Version = Version,

                Structure = structure ?? _defaultConfiguration.Structure,
                DisablePreview = disablePreview ?? _defaultConfiguration.DisablePreview,
                HideLabel = hideLabel ?? _defaultConfiguration.HideLabel,
                HidePropertyGroupContainer = hidePropertyGroupContainer ?? _defaultConfiguration.HidePropertyGroupContainer,
                AllowBlocksWithoutHeader = requireHeaderForBlocks ?? _defaultConfiguration.AllowBlocksWithoutHeader,
            };
        }

        public override Dictionary<string, object> ToConfigurationEditor(ContentBlocksConfiguration configuration)
        {
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

#if NET5_0_OR_GREATER

        public override object FromDatabase(string configuration, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
#elif NET472
        public override object FromDatabase(string configuration)
#endif
        {
            if (configuration == null || configuration.Trim() == "{}")
            {
                // Special case: empty configuration object.
                // That is not allowed, return the default.
                return _defaultConfiguration;
            }

            try
            {
#if NET5_0_OR_GREATER
                if (base.FromDatabase(configuration, configurationEditorJsonSerializer) is ContentBlocksConfiguration contentBlocksConfiguration)
#elif NET472
                if (base.FromDatabase(configuration) is ContentBlocksConfiguration contentBlocksConfiguration)
#endif
                {
                    return ApplyMigrations(contentBlocksConfiguration);
                }
                else
                {
                    return _defaultConfiguration;
                };
            }
            catch
            {
                return _defaultConfiguration;
            }
        }

        private ContentBlocksConfiguration ApplyMigrations(ContentBlocksConfiguration source)
        {
            if (source.Version == Version)
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

        private bool? GetBool(IDictionary<string, object> config, string key)
        {
            if (config != null && config.TryGetValue(key, out object value) &&
                ParseBoolean(value?.ToString()) is bool boolValue)
            {
                return boolValue;
            };

            return null;
        }

        private bool? ParseBoolean(string input)
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

        private Structure? GetStructure(IDictionary<string, object> config)
        {
            Structure structure = Structure.None;

            if (config != null &&
                config.TryGetValue(StructureKey, out object obj) &&
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
}
