using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlockVariantModelValue
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("alias")]
    public string Alias { get; set; } = "";

    [JsonPropertyName("content")]
    public JsonNode? Content { get; set; }
}
