using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValidator(
    IPropertyValidationService validationService, ContentBlocksModelValueDeserializer deserializer, ContentBlocksDataResolver resolver)
    : ComplexEditorValidator(validationService)
{
    protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value)
    {
        if (deserializer.Deserialize(value?.ToString()) is not ContentBlocksModelValue modelValue ||
            resolver.Resolve(modelValue) is not Dictionary<Guid, BlockItemData> data)
        {
            yield break;
        }

        foreach (var headerValidation in GetValidationModels(modelValue.Header, data))
        {
            yield return headerValidation;
        }

        foreach (var blockValidation in modelValue.Blocks?.SelectMany(block => GetValidationModels(block, data)) ?? [])
        {
            yield return blockValidation;
        }

        static IEnumerable<ElementTypeValidationModel> GetValidationModels(ContentBlockModelValue? block, Dictionary<Guid, BlockItemData> data)
        {
            if (block is null)
            {
                yield break;
            }

            if (data.TryGetValue(block.Id, out var blockData) &&
                Validate(blockData, block.Id.ToString()) is ElementTypeValidationModel validationModel)
            {
                yield return validationModel;
            }

            foreach (var variant in block.Variants ?? [])
            {
                if (data.TryGetValue(variant.Id, out var variantData) &&
                    Validate(variantData, block.Id.ToString()) is ElementTypeValidationModel variantValidationModel)
                {
                    yield return variantValidationModel;
                }
            }

            static ElementTypeValidationModel? Validate(BlockItemData? data, string jsonPathPrefix)
            {
                if (data is null ||
                    GetValidationModel(data, jsonPathPrefix) is not ElementTypeValidationModel validationModel)
                {
                    return null;
                }

                return validationModel;
            }
        }

        static ElementTypeValidationModel? GetValidationModel(BlockItemData block, string jsonPathPrefix)
        {
            var elementValidation = new ElementTypeValidationModel(block.ContentTypeAlias, block.Key);
            foreach (KeyValuePair<string, BlockItemData.BlockPropertyValue> prop in block.PropertyValues)
            {
                var propTypeValidation = new PropertyTypeValidationModel(prop.Value.PropertyType, prop.Value.Value, $"{jsonPathPrefix}.{prop.Value.PropertyType.Alias}");
                elementValidation.AddPropertyTypeValidation(propTypeValidation);
            }

            return elementValidation;
        }
    }
}
