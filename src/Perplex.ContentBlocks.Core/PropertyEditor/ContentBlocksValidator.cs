﻿using Perplex.ContentBlocks.PropertyEditor.Value;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValidator(
    IPropertyValidationService validationService, ContentBlocksValueDeserializer deserializer, ContentBlocksValueRefiner refiner)
    : ComplexEditorValidator(validationService)
{
    protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value)
    {
        if (deserializer.Deserialize(value?.ToString()) is not ContentBlocksValue model)
        {
            return [];
        }

        refiner.Refine(model);

        var validationModels = ContentBlocksValueIterator.Iterate(model,
            block => Validate(block.Content, block.Id.ToString()),
            variant => Validate(variant.Content, variant.Id.ToString()));

        return [.. validationModels.OfType<ElementTypeValidationModel>()];

        static ElementTypeValidationModel? Validate(BlockItemData? data, string jsonPathPrefix)
        {
            if (data is null)
            {
                return null;
            }

            var elementValidation = new ElementTypeValidationModel(data.ContentTypeAlias, data.Key);
            foreach (var prop in data.PropertyValues)
            {
                var propTypeValidation = new PropertyTypeValidationModel(prop.Value.PropertyType, prop.Value.Value, $"{jsonPathPrefix}.{prop.Value.PropertyType.Alias}");
                elementValidation.AddPropertyTypeValidation(propTypeValidation);
            }

            return elementValidation;
        }
    }
}
