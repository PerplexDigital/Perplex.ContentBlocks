namespace Perplex.ContentBlocks.DeliveryApi;
public class ApiContentBlocks
{
    public static readonly ApiContentBlocks Empty = new();

    public IApiContentBlockViewModel? Header { get; init; }
    public IEnumerable<IApiContentBlockViewModel> Blocks { get; init; } = [];
}
