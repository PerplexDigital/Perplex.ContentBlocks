using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Perplex.ContentBlocks.Api
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [BackOfficeRoute("perplexcontentblocks/api")]
    [MapToApi(Constants.Api.ApiName)]
    [JsonOptionsName(Umbraco.Cms.Core.Constants.JsonOptionsNames.BackOffice)]
    [Produces("application/json")]
    public abstract class ContentBlocksApiControllerBase : ControllerBase
    {
    }
}
