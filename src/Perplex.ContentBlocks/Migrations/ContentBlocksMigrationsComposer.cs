using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Features.Migrations
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Install)]
    public class ContentBlocksMigrationsComposer : ComponentComposer<ContentBlocksMigrationsComponent>
    {
    }
}
