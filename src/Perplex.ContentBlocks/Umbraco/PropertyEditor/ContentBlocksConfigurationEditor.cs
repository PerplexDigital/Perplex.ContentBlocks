using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.Umbraco.Configuration;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using static Perplex.ContentBlocks.Constants.Umbraco.Configuration;

namespace Perplex.ContentBlocks.Umbraco.PropertyEditor
{
    public class ContentBlocksConfigurationEditor : ConfigurationEditor<ContentBlocksConfiguration>
    {
        public ContentBlocksConfigurationEditor()
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
                    Name = "Disable Preview",
                    Description = "Disables the preview feature",
                    Key = DisablePreviewKey,
                    View = DisablePreviewViewName,
                },

                new ConfigurationField
                {
                    Name = "Hide Label",
                    Description = "Hides the data type label",
                    Key = HideLabelKey,
                    View = HideLabelViewName,
                },
            });
        }

        /// <summary>
        /// Current configuration version.
        /// </summary>
        public const int Version = 1;

        internal static readonly ContentBlocksConfiguration _defaultConfiguration = new ContentBlocksConfiguration
        {
            Version = Version,

            HideLabel = true,
            Structure = Structure.Blocks | Structure.Header,
            DisablePreview = false,
        };

        public override object DefaultConfigurationObject { get; }
            = _defaultConfiguration;

        public override IDictionary<string, object> DefaultConfiguration
            => ToConfigurationEditor(_defaultConfiguration);

        public override ContentBlocksConfiguration FromConfigurationEditor(IDictionary<string, object> editorValues, ContentBlocksConfiguration configuration)
        {
            var hideLabel = GetHideLabel(editorValues);
            var layout = GetStructure(editorValues);
            var disablePreview = GetDisablePreview(editorValues);

            return new ContentBlocksConfiguration
            {
                // When saved we store the current version,
                // Any transformations should have been applied before
                // and the user has now actively saved the configuration again.
                Version = Version,

                Structure = layout ?? _defaultConfiguration.Structure,
                DisablePreview = disablePreview ?? _defaultConfiguration.DisablePreview,
                HideLabel = hideLabel ?? _defaultConfiguration.HideLabel,
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
            };
        }

        public override object FromDatabase(string configuration)
        {
            if (configuration == null || configuration.Trim() == "{}")
            {
                // Special case: empty configuration object.
                // That is not allowed, return the default.
                return _defaultConfiguration;
            }

            try
            {
                return base.FromDatabase(configuration);
            }
            catch
            {
                return _defaultConfiguration;
            }
        }

        private bool? GetDisablePreview(IDictionary<string, object> config)
        {
            if (config != null && config.TryGetValue(DisablePreviewKey, out object value) &&
                ParseBoolean(value?.ToString()) is bool disablePreview)
            {
                return disablePreview;
            };

            return null;
        }

        private bool? GetHideLabel(IDictionary<string, object> config)
        {
            if (config != null && config.TryGetValue(HideLabelKey, out object value) &&
                ParseBoolean(value?.ToString()) is bool hideLabel)
            {
                return hideLabel;
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
