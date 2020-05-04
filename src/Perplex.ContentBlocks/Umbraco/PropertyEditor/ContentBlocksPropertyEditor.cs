using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Umbraco.ModelValue;
using Perplex.ContentBlocks.Umbraco.Validation;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.Umbraco.PropertyEditor
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

        public string Alias { get; } = Constants.Umbraco.PropertyEditor.Alias;
        public EditorType Type { get; } = EditorType.PropertyValue;
        public string Name { get; } = Constants.Umbraco.PropertyEditor.Name;
        public string Icon { get; }
        public string Group { get; }
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
            return new DataValueEditor(Constants.Umbraco.PropertyEditor.ViewPath, validator)
            {
                Configuration = configuration,
                HideLabel = true
            };
        }
    }
}
