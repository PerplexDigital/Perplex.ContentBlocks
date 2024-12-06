using System.Text.Json.Nodes;
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
            var value = jsonSerializer.Deserialize<JsonNode>(json);

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

    private ContentBlocksValue? MaybeTransformData(JsonNode value)
    {
        if (value["version"]?.GetValue<int>() is not int version)
        {
            return null;
        }

        if (version < 4)
        {
            // TODO:
            // v1 - v3: upgrade format to v4
        }

        return jsonSerializer.Deserialize<ContentBlocksValue>(value.ToString());
    }
}
