namespace Perplex.ContentBlocks.Presets;

public class ContentBlockPreset : IContentBlockPreset
{
    public required Guid Id { get; set; }
    public required Guid DefinitionId { get; set; }
    public required Guid LayoutId { get; set; }
    public bool IsMandatory { get; set; }

    public IDictionary<string, object> Values { get; set; }
        = new Dictionary<string, object>();
}
