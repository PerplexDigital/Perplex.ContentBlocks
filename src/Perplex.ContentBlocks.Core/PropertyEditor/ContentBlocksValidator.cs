using Perplex.ContentBlocks.PropertyEditor.Value;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValidator(
    IPropertyValidationService validationService, ContentBlocksValueDeserializer deserializer, ContentBlocksValueRefiner resolver)
    : ComplexEditorValidator(validationService)
{
    protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value)
    {
        if (deserializer.Deserialize(value?.ToString()) is not ContentBlocksValue model)
        {
            yield break;
        }

        resolver.Refine(model);

        foreach (var headerValidation in GetValidationModels(model.Header))
        {
            yield return headerValidation;
        }

        foreach (var blockValidation in model.Blocks?.SelectMany(block => GetValidationModels(block)) ?? [])
        {
            yield return blockValidation;
        }

        static IEnumerable<ElementTypeValidationModel> GetValidationModels(ContentBlockValue? block)
        {
            if (block is null)
            {
                yield break;
            }

            if (Validate(block.Content, block.Id.ToString()) is ElementTypeValidationModel validationModel)
            {
                yield return validationModel;
            }

            foreach (var variant in block.Variants ?? [])
            {
                if (Validate(variant.Content, block.Id.ToString()) is ElementTypeValidationModel variantValidationModel)
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
