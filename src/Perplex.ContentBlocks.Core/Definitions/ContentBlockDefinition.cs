namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinition : IContentBlockDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string BlockNameTemplate { get; set; } = "";
    public string PreviewImage { get; set; } = "";
    public Guid ElementTypeKey { get; set; }
    public IEnumerable<Guid> CategoryIds { get; set; } = [];

    public IEnumerable<IContentBlockLayout> Layouts { get; set; } = [];

    public IEnumerable<string> LimitToDocumentTypes { get; set; } = [];

    public IEnumerable<string> LimitToCultures { get; set; } = [];
}
