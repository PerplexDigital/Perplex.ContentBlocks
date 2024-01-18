using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValidator : ComplexEditorValidator
{
    private readonly ContentBlockUtils _utils;
    private readonly ContentBlocksModelValueDeserializer _deserializer;

    private readonly IShortStringHelper _shortStringHelper;

    public ContentBlocksValidator(
        ContentBlocksModelValueDeserializer deserializer,
        ContentBlockUtils utils,
        IPropertyValidationService validationService,
        IShortStringHelper shortStringHelper) : base(validationService)
    {
        _deserializer = deserializer;
        _utils = utils;
        _shortStringHelper = shortStringHelper;
    }

    protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value)
    {
        var modelValue = _deserializer.Deserialize(value?.ToString());
        if (modelValue == null)
        {
            yield break;
        }

        if (modelValue.Header != null && GetValidationModel(modelValue.Header) is ElementTypeValidationModel headerValidationModel)
        {
            yield return headerValidationModel;
        }

        if (modelValue.Blocks?.Any() == true)
        {
            foreach (var block in modelValue.Blocks)
            {
                if (GetValidationModel(block) is ElementTypeValidationModel blockValidationModel)
                {
                    yield return blockValidationModel;
                }
            }
        }

        ElementTypeValidationModel? GetValidationModel(ContentBlockModelValue blockValue)
        {
            IDataType? dataType = _utils.GetDataType(blockValue.DefinitionId);

            if (dataType is null)
            {
                return null;
            }

            var validationModel = new ElementTypeValidationModel("", blockValue.Id);

            var propType = new PropertyType(_shortStringHelper, dataType) { Alias = "content" };
            validationModel.AddPropertyTypeValidation(new PropertyTypeValidationModel(propType, blockValue.Content?.ToString()));

            if (blockValue.Variants?.Any() == true)
            {
                foreach (var variant in blockValue.Variants)
                {
                    var variantPropType = new PropertyType(_shortStringHelper, dataType) { Alias = "content_variant_" + variant.Id.ToString("N") };
                    validationModel.AddPropertyTypeValidation(new PropertyTypeValidationModel(variantPropType, variant.Content?.ToString()));
                }
            }

            return validationModel;
        }
    }
}
