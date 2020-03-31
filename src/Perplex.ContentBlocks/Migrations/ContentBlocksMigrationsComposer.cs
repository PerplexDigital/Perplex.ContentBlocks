using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Features.Migrations
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksMigrationsComposer : ComponentComposer<ContentBlocksMigrationsComponent>
    {
    }
}
