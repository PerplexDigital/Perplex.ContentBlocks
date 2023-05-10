#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
#elif NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif


namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
#if NET6_0_OR_GREATER
    public class ContentBlocksModelValueComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ContentBlocksModelValueDeserializer>();
            builder.AddNotificationHandler<ContentCopyingNotification, ContentBlocksModelValueCopyingHandler>();
        }
    }
#elif NETFRAMEWORK
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
