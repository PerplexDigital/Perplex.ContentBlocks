#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
#if NET5_0
    public class ContentBlocksModelValueComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ContentBlocksModelValueDeserializer>();
            builder.AddNotificationHandler<ContentCopyingNotification, ContentBlocksModelValueCopyingHandler>();
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class ContentBlocksModelValueComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ContentBlocksModelValueDeserializer>(Lifetime.Singleton);
            composition.Components().Append<ContentBlocksModelValueComponent>();
        }
    }
#endif
}
