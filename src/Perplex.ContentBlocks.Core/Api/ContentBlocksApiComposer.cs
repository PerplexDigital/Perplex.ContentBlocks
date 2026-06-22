using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

#if NET9_0
using OpenApiInfo = Microsoft.OpenApi.Models.OpenApiInfo;
#elif NET10_0_OR_GREATER
using OpenApiInfo = Microsoft.OpenApi.OpenApiInfo;
#endif

namespace Perplex.ContentBlocks.Api;

public class ContentBlocksApiComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IStartupFilter, ContentBlocksApiStartupFilter>();

        builder.Services.Configure<SwaggerGenOptions>(options =>
        {
            options.SwaggerDoc(Constants.Api.ApiName, new OpenApiInfo
            {
                Title = "Perplex.ContentBlocks API",
            });

            options.OperationFilter<ContentBlocksApiSecurityFilter>();
        });
    }
}
