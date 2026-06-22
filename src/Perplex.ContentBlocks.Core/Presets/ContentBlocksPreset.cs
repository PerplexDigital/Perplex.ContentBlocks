namespace Perplex.ContentBlocks.Presets;

public class ContentBlocksPreset : IContentBlocksPreset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public IEnumerable<string> ApplyToCultures { get; set; } = [];

    public IEnumerable<string> ApplyToDocumentTypes { get; set; } = [];

    public IContentBlockPreset? Header { get; set; }

    public IEnumerable<IContentBlockPreset> Blocks { get; set; } = [];
}
