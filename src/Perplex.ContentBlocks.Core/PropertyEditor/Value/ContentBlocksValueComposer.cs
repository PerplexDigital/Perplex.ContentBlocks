using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksValueComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<ContentBlocksValueDeserializer>();
        builder.AddNotificationHandler<ContentCopyingNotification, ContentBlocksValueCopyingHandler>();
    }
}
