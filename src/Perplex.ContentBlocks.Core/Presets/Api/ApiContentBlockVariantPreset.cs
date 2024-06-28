namespace Perplex.ContentBlocks.Presets.Api;

public class ApiContentBlockVariantPreset
{
    public required Guid Id { get; set; }

    public required string Alias { get; set; }

    public required IDictionary<string, object> Values { get; set; }
}
