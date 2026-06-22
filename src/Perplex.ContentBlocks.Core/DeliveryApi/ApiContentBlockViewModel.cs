using Umbraco.Cms.Core.Models.DeliveryApi;

namespace Perplex.ContentBlocks.DeliveryApi;

public class ApiContentBlockViewModel : IApiContentBlockViewModel
{
    public required Guid Id { get; init; }

    public required Guid DefinitionId { get; init; }

    public required Guid LayoutId { get; init; }

    public IApiElement? Content { get; init; }
}
