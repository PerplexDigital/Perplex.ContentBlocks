namespace Perplex.ContentBlocks.DeliveryApi;
public interface IApiContentBlocks
{
    IApiContentBlockViewModel Header { get; }
    IEnumerable<IApiContentBlockViewModel> Blocks { get; }
}
