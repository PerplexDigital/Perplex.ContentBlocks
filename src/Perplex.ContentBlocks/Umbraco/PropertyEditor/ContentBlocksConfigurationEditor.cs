using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.Umbraco.Configuration;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

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
                    Name = "Editor layout",
                    Description = "Set editor layout",
                    Key = "layout",
                    View = Constants.Umbraco.Configuration.EditorLayoutEditorViewName,
                },

                new ConfigurationField
                {
                    Name = "Disable Preview",
                    Description = "Completely disables the Preview feature",
                    Key = "disablePreview",
                    View = Constants.Umbraco.Configuration.DisablePreviewViewName,
                },
            });
        }

        private static readonly ContentBlocksConfiguration _defaultConfiguration = new ContentBlocksConfiguration
        {
            DisablePreview = false,
            Layout = EditorLayout.Blocks | EditorLayout.Header
        };

        public override object DefaultConfigurationObject { get; }
            = _defaultConfiguration;

        public override IDictionary<string, object> DefaultConfiguration
            => ToConfigurationEditor(_defaultConfiguration);

        public override ContentBlocksConfiguration FromConfigurationEditor(IDictionary<string, object> editorValues, ContentBlocksConfiguration configuration)
        {
            var layout = GetEditorLayout(editorValues);
            var disablePreview = GetDisablePreview(editorValues);

            return new ContentBlocksConfiguration
            {
                Layout = layout ?? _defaultConfiguration.Layout,
                DisablePreview = disablePreview ?? _defaultConfiguration.DisablePreview,
            };
        }

        public override Dictionary<string, object> ToConfigurationEditor(ContentBlocksConfiguration configuration)
        {
            return new Dictionary<string, object>
            {
                [Constants.Umbraco.Configuration.EditorLayoutKey] = new
                {
                    blocks = configuration.Layout.HasFlag(EditorLayout.Blocks),
                    header = configuration.Layout.HasFlag(EditorLayout.Header),
                },

                [Constants.Umbraco.Configuration.DisablePreviewKey] = configuration.DisablePreview,
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
            if (config != null && config.TryGetValue(Constants.Umbraco.Configuration.DisablePreviewKey, out object configuredLayout) &&
                bool.TryParse(configuredLayout.ToString(), out bool disablePreview))
            {
                return disablePreview;
            };

            return null;
        }

        private EditorLayout? GetEditorLayout(IDictionary<string, object> config)
        {
            EditorLayout layout = EditorLayout.None;

            if (config != null &&
                config.TryGetValue(Constants.Umbraco.Configuration.EditorLayoutKey, out object obj) &&
                obj is JObject configuredLayout)
            {
                if (configuredLayout.Value<bool>("blocks"))
                    layout |= EditorLayout.Blocks;

                if (configuredLayout.Value<bool>("header"))
                    layout |= EditorLayout.Header;

                return layout;
            };

            return null;
        }
    }
}
