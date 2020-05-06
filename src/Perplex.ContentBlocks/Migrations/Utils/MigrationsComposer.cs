using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Migrations.Utils
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Upgrade)]
    public class MigrationsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IMigrationPlanExecuter, MigrationPlanExecuter>(Lifetime.Transient);
        }
    }
}
