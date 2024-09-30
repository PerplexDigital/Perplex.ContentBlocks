using Umbraco.Cms.Core.Models.DeliveryApi;

namespace Perplex.ContentBlocks.DeliveryApi;

public class ApiContentBlockViewModel : IApiContentBlockViewModel
{
    public Guid Id { get; init; }

    public Guid DefinitionId { get; init; }

    public Guid LayoutId { get; init; }

    public IApiElement? Content { get; init; }
}
