using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlockModelValue
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("definitionId")]
    public Guid DefinitionId { get; set; }

    [JsonPropertyName("layoutId")]
    public Guid LayoutId { get; set; }

    [JsonPropertyName("presetId")]
    public Guid? PresetId { get; set; }

    [JsonPropertyName("isDisabled")]
    public bool IsDisabled { get; set; }

    [JsonPropertyName("content")]
    public JsonNode? Content { get; set; }

    [JsonPropertyName("variants")]
    public List<ContentBlockVariantModelValue>? Variants { get; set; }
}
