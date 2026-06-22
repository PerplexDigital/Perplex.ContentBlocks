namespace Perplex.ContentBlocks.Categories;

public class ContentBlockCategory : IContentBlockCategory
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string Icon { get; set; } = "";
    public bool IsHidden { get; set; }
    public bool IsEnabledForHeaders { get; set; }
    public bool IsDisabledForBlocks { get; set; }
}
