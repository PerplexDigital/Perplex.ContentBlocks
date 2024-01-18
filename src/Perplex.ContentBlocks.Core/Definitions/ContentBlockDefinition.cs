namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinition : IContentBlockDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string PreviewImage { get; set; } = "";
    public int? DataTypeId { get; set; }
    public Guid? DataTypeKey { get; set; }

    public IEnumerable<Guid> CategoryIds { get; set; }
         = Array.Empty<Guid>();

    public IEnumerable<IContentBlockLayout> Layouts { get; set; }
        = Array.Empty<IContentBlockLayout>();

    public virtual IEnumerable<string> LimitToDocumentTypes { get; set; }
        = Array.Empty<string>();

    public virtual IEnumerable<string> LimitToCultures { get; set; }
        = Array.Empty<string>();
}
