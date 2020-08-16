using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksPropertyEditor : IDataEditor
    {
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly ContentBlockUtils _utils;

        public ContentBlocksPropertyEditor(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils)
        {
            _deserializer = deserializer;
            _utils = utils;
        }

        public string Alias { get; } = Constants.PropertyEditor.Alias;
        public EditorType Type { get; } = EditorType.PropertyValue;
        public string Name { get; } = Constants.PropertyEditor.Name;

        // Icon cannot be NULL for Umbraco 8.6+,
        // it will actually crash the UI.
        public string Icon { get; } = "icon-list";

        public string Group { get; } = "Lists";

        public bool IsDeprecated { get; } = false;
        public IDictionary<string, object> DefaultConfiguration { get; }

        public IPropertyIndexValueFactory PropertyIndexValueFactory
            => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor()
            => new ContentBlocksConfigurationEditor();

        public IDataValueEditor GetValueEditor()
            => GetValueEditor(null);

        public IDataValueEditor GetValueEditor(object configuration)
        {
            var validator = new ContentBlocksValidator(_deserializer, _utils);

            bool hideLabel = (configuration as ContentBlocksConfiguration)?.HideLabel
                ?? ContentBlocksConfigurationEditor._defaultConfiguration.HideLabel;

            return new ContentBlocksValueEditor(Constants.PropertyEditor.ViewPath, _deserializer, _utils, validator)
            {
                Configuration = configuration,
                HideLabel = hideLabel,
                ValueType = ValueTypes.Json,
            };
        }
    }
}
