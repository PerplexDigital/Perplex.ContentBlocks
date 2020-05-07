using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksPropertyEditor : IDataEditor
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentBlockDefinitionRepository _contentBlockDefinitionRepository;
        private readonly ContentBlocksModelValueDeserializer _contentBlocksModelValueDeserializer;

        public ContentBlocksPropertyEditor(
            IDataTypeService dataTypeService,
            IContentBlockDefinitionRepository contentBlockDefinitionRepository,
            ContentBlocksModelValueDeserializer contentBlocksModelValueDeserializer)
        {
            _dataTypeService = dataTypeService;
            _contentBlockDefinitionRepository = contentBlockDefinitionRepository;
            _contentBlocksModelValueDeserializer = contentBlocksModelValueDeserializer;
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
            var validator = new ContentBlocksValidator(_dataTypeService, _contentBlockDefinitionRepository, _contentBlocksModelValueDeserializer);

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
