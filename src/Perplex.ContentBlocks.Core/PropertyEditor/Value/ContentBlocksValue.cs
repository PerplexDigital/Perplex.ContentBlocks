using System.Text.Json.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksValue
{
    /// <summary>
    /// The current model value version
    /// </summary>
    public const int CurrentVersion = 4;

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("header")]
    public ContentBlockValue? Header { get; set; }

    [JsonPropertyName("blocks")]
    public List<ContentBlockValue>? Blocks { get; set; }
}
