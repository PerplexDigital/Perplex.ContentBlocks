namespace Perplex.ContentBlocks.Categories.Api;
public class ApiContentBlockCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Icon { get; set; }
    public bool IsHidden { get; set; }
    public bool IsEnabledForHeaders { get; set; }
    public bool IsDisabledForBlocks { get; set; }
}
