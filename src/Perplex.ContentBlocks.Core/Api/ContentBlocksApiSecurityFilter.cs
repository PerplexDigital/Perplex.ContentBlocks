using Umbraco.Cms.Api.Management.OpenApi;

namespace Perplex.ContentBlocks.Api;

internal sealed class ContentBlocksApiSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase
{
    protected override string ApiName => Constants.Api.ApiName;
}
