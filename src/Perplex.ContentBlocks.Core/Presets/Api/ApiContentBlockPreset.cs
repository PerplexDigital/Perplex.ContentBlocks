namespace Perplex.ContentBlocks.Presets.Api;

public class ApiContentBlockPreset
{
    public required Guid Id { get; set; }
    public required Guid DefinitionId { get; set; }
    public required Guid LayoutId { get; set; }
    public bool IsMandatory { get; set; }
    public required IDictionary<string, object> Values { get; set; }
}
