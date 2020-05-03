using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Umbraco.ModelValue;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.Umbraco.Validation
{
    public class ContentBlocksValidationComponentComposer : ComponentComposer<ContentBlocksValidationComponent>
    {
    }

    public class ContentBlocksValidationComponent : IComponent
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentBlockDefinitionRepository _contentBlockDefinitionRepository;
        private readonly ContentBlocksModelValueDeserializer _deserializer;

        public ContentBlocksValidationComponent(
            IDataTypeService dataTypeService,
            IContentBlockDefinitionRepository contentBlockDefinitionRepository,
            ContentBlocksModelValueDeserializer deserializer)
        {
            _dataTypeService = dataTypeService;
            _contentBlockDefinitionRepository = contentBlockDefinitionRepository;
            _deserializer = deserializer;
        }

        public void Initialize()
        {
            var contentBlocksDataTypes = _dataTypeService.GetByEditorAlias(Constants.Umbraco.ContentBlocksPropertyEditorAlias);

            var validator = new ContentBlocksValidator(_dataTypeService, _contentBlockDefinitionRepository, _deserializer);
            foreach (IDataType contentBlocksDataType in contentBlocksDataTypes)
            {
                var valueEditor = contentBlocksDataType.Editor.GetValueEditor();
                valueEditor.Validators.Add(validator);
            };
        }

        public void Terminate()
        {
        }
    }
}
