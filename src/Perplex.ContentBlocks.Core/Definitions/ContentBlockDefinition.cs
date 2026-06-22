namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinition : IContentBlockDefinition
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid ElementTypeKey { get; set; }
    public required IEnumerable<Guid> CategoryIds { get; set; }
    public required IEnumerable<IContentBlockLayout> Layouts { get; set; }

    public string Description { get; set; } = "";
    public string BlockNameTemplate { get; set; } = "";
    public string Icon { get; set; } = "";
    public string PreviewImage { get; set; } = "";

    public IEnumerable<string> LimitToDocumentTypes { get; set; } = [];
    public IEnumerable<string> LimitToCultures { get; set; } = [];
}
