using Umbraco.Cms.Core.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksValueDeserializer(IJsonSerializer jsonSerializer)
{
    /// <summary>
    /// Deserializes the given JSON to an instance of <see cref="ContentBlocksValue"/>.
    /// </summary>
    /// <param name="json">JSON to deserialize</param>
    /// <returns></returns>
    public ContentBlocksValue? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var value = jsonSerializer.Deserialize<ContentBlocksValue>(json);

            if (value is null)
            {
                return null;
            }

            return MaybeTransformData(value);
        }
        catch
        {
            return null;
        }
    }

    private static ContentBlocksValue MaybeTransformData(ContentBlocksValue value)
    {
        if (value.Version < 3)
        {
            // We added a Variants property in v3, for any older version we will ensure this property becomes an empty Array.
            if (value.Header != null && value.Header.Variants == null)
            {
                value.Header.Variants = [];
            }

            if (value.Blocks != null)
            {
                foreach (var block in value.Blocks)
                {
                    block.Variants ??= [];
                }
            }
        }

        return value;
    }
}
