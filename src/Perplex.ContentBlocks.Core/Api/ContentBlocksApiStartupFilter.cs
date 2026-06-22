using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Perplex.ContentBlocks.Api;
public class ContentBlocksApiStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseMiddleware<BearerTokenFromQueryStringMiddleware>();
            next(builder);
        };
    }
}
