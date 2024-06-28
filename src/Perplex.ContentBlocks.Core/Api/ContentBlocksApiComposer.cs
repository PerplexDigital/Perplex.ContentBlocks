using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.Api;
public class ContentBlocksApiComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IStartupFilter, ContentBlocksApiStartupFilter>();
    }
}
