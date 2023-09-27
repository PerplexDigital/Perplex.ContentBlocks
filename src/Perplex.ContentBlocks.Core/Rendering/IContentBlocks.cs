namespace Perplex.ContentBlocks.Rendering;

public interface IContentBlocks
{
    IContentBlockViewModel? Header { get; }
    IEnumerable<IContentBlockViewModel> Blocks { get; }
}
