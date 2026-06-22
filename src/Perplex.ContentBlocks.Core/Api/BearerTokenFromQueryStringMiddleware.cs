using Microsoft.AspNetCore.Http;
using static Umbraco.Cms.Core.Constants.System;

namespace Perplex.ContentBlocks.Api;

/// <summary>
/// Copies the value of the query string parameter "token" into the header "Authorization: Bearer {token}"
/// This is useful when only a basic GET request can be issued but authentication is required in the header.
/// It will only be applied to requests to Perplex.ContentBlocks API endpoints.
/// The main use case is the Preview API that is accessed directly from an iframe using the src attribute which
/// issues a basic GET request.
/// </summary>
/// <param name="next">The next middleware</param>
public class BearerTokenFromQueryStringMiddleware(RequestDelegate next)
{
    private static readonly string _pathPrefix = DefaultUmbracoPath.TrimStart('~') + "/" + Constants.Api.BaseRoute;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(_pathPrefix) &&
            context.Request.Query.TryGetValue(Constants.Api.TokenQueryStringKey, out var token) &&
            !context.Request.Headers.ContainsKey("Authorization") &&
            !string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
        }

        await next(context);
    }
}
