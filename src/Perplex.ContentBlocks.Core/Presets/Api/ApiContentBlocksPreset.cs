namespace Perplex.ContentBlocks.Presets.Api;

public class ApiContentBlocksPreset
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string[] ApplyToCultures { get; set; }
    public required string[] ApplyToDocumentTypes { get; set; }
    public required ApiContentBlockPreset? Header { get; set; }
    public required ApiContentBlockPreset[] Blocks { get; set; }
}
