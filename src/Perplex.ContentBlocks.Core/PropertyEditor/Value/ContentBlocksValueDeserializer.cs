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
            return jsonSerializer.Deserialize<ContentBlocksValue>(json);
        }
        catch
        {
            return null;
        }
    }
}
