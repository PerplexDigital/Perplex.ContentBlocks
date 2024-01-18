namespace Perplex.ContentBlocks.Presets;

public class ContentBlockVariantPreset : IContentBlockVariantPreset
{
    public Guid Id { get; set; }

    public string Alias { get; set; } = "";

    /// <summary>
    /// The initial values of the generated Content Block variant per property alias of the IPublishedElement
    /// </summary>
    public IDictionary<string, object> Values { get; set; }
        = new Dictionary<string, object>();
}
