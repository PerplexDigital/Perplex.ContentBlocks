using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlocksModelValueComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<ContentBlocksModelValueDeserializer>();
        builder.Services.AddSingleton<ContentBlocksBlockContentConverter>();
        builder.AddNotificationHandler<ContentCopyingNotification, ContentBlocksModelValueCopyingHandler>();
    }
}
