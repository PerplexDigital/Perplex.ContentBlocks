using Microsoft.AspNetCore.Mvc;

namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinition<T> : IContentBlockDefinition<T> where T : ViewComponent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string BlockNameTemplate { get; set; } = "";
    public string Icon { get; set; } = "";
    public string PreviewImage { get; set; } = "";
    public Guid ElementTypeKey { get; set; }

    public IEnumerable<Guid> CategoryIds { get; set; } = [];

    public IEnumerable<IContentBlockLayout> Layouts { get; set; } = [];

    public virtual IEnumerable<string> LimitToDocumentTypes { get; set; } = [];

    public virtual IEnumerable<string> LimitToCultures { get; set; } = [];
}
