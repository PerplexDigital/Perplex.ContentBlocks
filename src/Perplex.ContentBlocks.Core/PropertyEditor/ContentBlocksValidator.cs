using Perplex.ContentBlocks.PropertyEditor.Value;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.Validation;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValidator(
    IPropertyValidationService validationService, ContentBlocksValueDeserializer deserializer)
    : ComplexEditorValidator(validationService)
{
    protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value, PropertyValidationContext validationContext)
    {
        if (deserializer.Deserialize(value?.ToString()) is not ContentBlocksValue model)
        {
            return [];
        }

        ContentBlockValue?[] blocks = [
            model.Header,
            .. model.Blocks ?? []
        ];

        return blocks
            .OfType<ContentBlockValue>()
            .Select(block => Validate(block.Content, block.Id.ToString()))
            .OfType<ElementTypeValidationModel>()
            .ToArray();

        static ElementTypeValidationModel? Validate(BlockItemData? data, string jsonPathPrefix)
        {
            if (data is null)
            {
                return null;
            }

            var elementValidation = new ElementTypeValidationModel(data.ContentTypeAlias, data.Key);
            foreach (var prop in data.Values)
            {
                if (prop.PropertyType is null)
                {
                    continue;
                }

                var propTypeValidation = new PropertyTypeValidationModel(prop.PropertyType, prop.Value, $"{jsonPathPrefix}.{prop.PropertyType.Alias}");
                elementValidation.AddPropertyTypeValidation(propTypeValidation);
            }

            return elementValidation;
        }
    }
}
