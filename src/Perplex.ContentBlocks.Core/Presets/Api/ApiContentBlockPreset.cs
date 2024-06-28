namespace Perplex.ContentBlocks.Presets.Api;

public class ApiContentBlockPreset
{
    public Guid Id { get; set; }
    public Guid DefinitionId { get; set; }
    public Guid LayoutId { get; set; }
    public bool IsMandatory { get; set; }
    public required IDictionary<string, object> Values { get; set; }
    public required ApiContentBlockVariantPreset[] Variants { get; set; }
}
