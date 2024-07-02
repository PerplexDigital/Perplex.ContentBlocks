using System.Text.Json.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksValue
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("header")]
    public ContentBlockValue? Header { get; set; }

    [JsonPropertyName("blocks")]
    public List<ContentBlockValue>? Blocks { get; set; }
}
