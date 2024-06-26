using System.Text.Json.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlocksModelValue
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("header")]
    public ContentBlockModelValue? Header { get; set; }

    [JsonPropertyName("blocks")]
    public List<ContentBlockModelValue>? Blocks { get; set; }
}
