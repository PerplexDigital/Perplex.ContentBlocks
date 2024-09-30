using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Perplex.ContentBlocks.PropertyEditor;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.DeliveryApi;
public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.PropertyValueConverters().Remove<ContentBlocksValueConverter>();

        // We use the original value converter in the Delivery API value converter so it needs to be registered.
        builder.Services
            .RemoveAll<ContentBlocksValueConverter>()
            .AddSingleton<ContentBlocksValueConverter>();
    }
}
