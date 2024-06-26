using Umbraco.Cms.Core.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlocksModelValueDeserializer(IJsonSerializer jsonSerializer)
{
    /// <summary>
    /// Deserializes the given JSON to an instance of ContentBlocksModelValue
    /// </summary>
    /// <param name="json">JSON to deserialize</param>
    /// <returns></returns>
    public ContentBlocksModelValue? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var modelValue = jsonSerializer.Deserialize<ContentBlocksModelValue>(json);
            if (modelValue is null) return null;
            return MaybeTransformData(modelValue);
        }
        catch
        {
            return null;
        }
    }

    private static ContentBlocksModelValue MaybeTransformData(ContentBlocksModelValue modelValue)
    {
        if (modelValue.Version < 3)
        {
            // We added a Variants property in v3, for any older version we will ensure this property becomes an empty Array.
            if (modelValue.Header != null && modelValue.Header.Variants == null)
            {
                modelValue.Header.Variants = [];
            }

            if (modelValue.Blocks != null)
            {
                foreach (var block in modelValue.Blocks)
                {
                    block.Variants ??= [];
                }
            }
        }

        return modelValue;
    }
}
