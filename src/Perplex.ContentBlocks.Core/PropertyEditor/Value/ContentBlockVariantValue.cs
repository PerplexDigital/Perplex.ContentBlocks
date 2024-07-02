using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models.Blocks;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlockVariantValue
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("alias")]
    public string Alias { get; set; } = "";

    [JsonPropertyName("content")]
    public BlockItemData? Content { get; set; }
}
