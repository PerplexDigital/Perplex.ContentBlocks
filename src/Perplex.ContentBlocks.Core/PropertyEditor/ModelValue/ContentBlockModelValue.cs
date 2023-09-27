using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlockModelValue
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("definitionId")]
    public Guid DefinitionId { get; set; }

    [JsonProperty("layoutId")]
    public Guid LayoutId { get; set; }

    /// <summary>
    /// Indien dit blok uit een preset komt zal dit een waarde hebben
    /// en wijzen naar de betreffende IContentBlockPreset
    /// </summary>
    [JsonProperty("presetId")]
    public Guid? PresetId { get; set; }

    [JsonProperty("isDisabled")]
    public bool IsDisabled { get; set; }

    /// <summary>
    /// JSON NestedContent
    /// </summary>
    [JsonProperty("content")]
    public JArray? Content { get; set; }

    [JsonProperty("variants")]
    public List<ContentBlockVariantModelValue>? Variants { get; set; }
}
