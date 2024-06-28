namespace Perplex.ContentBlocks.Definitions.Api;
public class ApiContentBlockDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string PreviewImage { get; set; } = "";
    public Guid ElementTypeKey { get; set; }
    public IEnumerable<Guid> CategoryIds { get; set; } = [];
    public IEnumerable<ApiContentBlockLayout> Layouts { get; set; } = [];
    public IEnumerable<string> LimitToDocumentTypes { get; set; } = [];
    public IEnumerable<string> LimitToCultures { get; set; } = [];
}
