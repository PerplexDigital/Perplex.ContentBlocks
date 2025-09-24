namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockLayout : IContentBlockLayout
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? PreviewImage { get; set; }
    public string? ViewPath { get; set; }
}
