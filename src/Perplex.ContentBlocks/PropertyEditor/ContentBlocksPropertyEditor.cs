using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System.Collections.Generic;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksPropertyEditor : IDataEditor
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly IFactory _factory;

        public ContentBlocksPropertyEditor(
            IDataTypeService dataTypeService,
            ContentBlocksModelValueDeserializer deserializer,
            IFactory factory)
        {
            _dataTypeService = dataTypeService;
            _deserializer = deserializer;
            _factory = factory;
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
            var validator = new ContentBlocksValidator(_dataTypeService, _deserializer, _factory);

            bool hideLabel = (configuration as ContentBlocksConfiguration)?.HideLabel
                ?? ContentBlocksConfigurationEditor._defaultConfiguration.HideLabel;

            return new DataValueEditor(Constants.PropertyEditor.ViewPath, validator)
            {
                Configuration = configuration,
                HideLabel = hideLabel,
                ValueType = ValueTypes.Json,
            };
        }
    }
}
