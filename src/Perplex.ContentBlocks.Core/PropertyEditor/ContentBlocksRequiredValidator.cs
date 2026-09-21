using Perplex.ContentBlocks.PropertyEditor.Value;
using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Core.PropertyEditors.Validators;

namespace Perplex.ContentBlocks.PropertyEditor;

/// <summary>
/// Validates that a mandatory Content Blocks property holds at least one block.
/// </summary>
/// <remarks>
/// A Content Blocks value is a versioned envelope, so a property without any blocks still reads as
/// <c>{"version":4,"header":null,"blocks":[]}</c>. Umbraco's <see cref="RequiredValidator"/> treats only
/// <c>{}</c> and <c>[]</c> as empty JSON, so an empty value is normalized before it is validated.
/// </remarks>
public class ContentBlocksRequiredValidator(ContentBlocksValueDeserializer deserializer) : RequiredValidator
{
    private const string EmptyJson = "{}";

    /// <inheritdoc />
    public override IEnumerable<ValidationResult> ValidateRequired(object? value, string? valueType)
        => base.ValidateRequired(IsEmpty(value) ? EmptyJson : value, valueType);

    private bool IsEmpty(object? value)
    {
        if (deserializer.Deserialize(value?.ToString()) is not ContentBlocksValue model)
        {
            return false;
        }

        return model.Header is null && model.Blocks is not { Count: > 0 };
    }
}
