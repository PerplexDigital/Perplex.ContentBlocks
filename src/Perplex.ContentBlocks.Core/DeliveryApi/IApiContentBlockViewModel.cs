using Umbraco.Cms.Core.Models.DeliveryApi;

namespace Perplex.ContentBlocks.DeliveryApi;

public interface IApiContentBlockViewModel
{
    Guid Id { get; }

    Guid DefinitionId { get; }

    Guid LayoutId { get; }

    IApiElement? Content { get; }
}
