using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValidator(
    IPropertyValidationService validationService, ContentBlocksBlockContentConverter converter,
    IContentTypeService contentTypeService, ContentBlocksModelValueDeserializer deserializer)
    : ComplexEditorValidator(validationService)
{
    protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value)
    {
        if (deserializer.Deserialize(value?.ToString()) is not ContentBlocksModelValue modelValue)
        {
            yield break;
        }

        var contentTypeKeys = GetContentTypeKeys(modelValue);

        var allElementTypes = contentTypeService.GetAll(contentTypeKeys).ToDictionary(b => b.Key);

        foreach (var headerValidation in GetValidationModels(modelValue.Header, allElementTypes, converter))
        {
            yield return headerValidation;
        }

        foreach (var blockValidation in modelValue.Blocks?.SelectMany(block => GetValidationModels(block, allElementTypes, converter)) ?? [])
        {
            yield return blockValidation;
        }

        static IEnumerable<ElementTypeValidationModel> GetValidationModels(ContentBlockModelValue? block, Dictionary<Guid, IContentType> allElementTypes, ContentBlocksBlockContentConverter converter)
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
                if (Validate(block.Content, block.Id.ToString()) is ElementTypeValidationModel variantValidationModel)
                {
                    yield return variantValidationModel;
                }
            }

            ElementTypeValidationModel? Validate(JsonNode? content, string jsonPathPrefix)
            {
                if (content is null ||
                    converter.ConvertToBlockItemData(content) is not BlockItemData data ||
                    GetValidationModel(data, jsonPathPrefix, allElementTypes) is not ElementTypeValidationModel validationModel)
                {
                    return null;
                }

                return validationModel;
            }
        }

        static ElementTypeValidationModel? GetValidationModel(BlockItemData block, string jsonPathPrefix, Dictionary<Guid, IContentType> allElementTypes)
        {
            if (!allElementTypes.TryGetValue(block.ContentTypeKey, out IContentType? elementType))
            {
                return null;
            }

            EnsureProperties(block, elementType);

            var elementValidation = new ElementTypeValidationModel(block.ContentTypeAlias, block.Key);
            foreach (KeyValuePair<string, BlockItemData.BlockPropertyValue> prop in block.PropertyValues)
            {
                var propTypeValidation = new PropertyTypeValidationModel(prop.Value.PropertyType, prop.Value.Value, $"{jsonPathPrefix}.{prop.Value.PropertyType.Alias}");
                elementValidation.AddPropertyTypeValidation(propTypeValidation);
            }

            return elementValidation;
        }

        static void EnsureProperties(BlockItemData block, IContentType elementType)
        {
            block.ContentTypeAlias = elementType.Alias;

            foreach (IPropertyType propType in elementType.CompositionPropertyTypes)
            {
                if (block.PropertyValues.ContainsKey(propType.Alias))
                {
                    continue;
                }

                if (block.RawPropertyValues.TryGetValue(propType.Alias, out var rawValue))
                {
                    // Raw value exists, use it
                    block.PropertyValues[propType.Alias] = new BlockItemData.BlockPropertyValue(rawValue, propType);
                    continue;
                }

                // No value exists, ensure we add a NULL value for both.
                block.PropertyValues[propType.Alias] = new BlockItemData.BlockPropertyValue(null, propType);
                block.RawPropertyValues[propType.Alias] = null;
            }
        }
    }

    private static Guid[] GetContentTypeKeys(ContentBlocksModelValue model)
    {
        var keys = new List<Guid>();

        if (ParseGuid(model.Header?.Content) is Guid headerKey)
        {
            keys.Add(headerKey);
        }

        foreach (var block in model.Blocks ?? [])
        {
            if (ParseGuid(block.Content) is Guid blockKey &&
                !keys.Contains(blockKey))
            {
                keys.Add(blockKey);
            }
        }

        return [.. keys];

        static Guid? ParseGuid(JsonNode? content) => content?["contentTypeKey"]?.GetValue<Guid?>();
    }
}
