using Athlon.Infrastructure.Migrations;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Features.Migrations
{
    public class ContentBlocksMigrationsComponent : IComponent
    {
        private readonly IMigrationPlanExecuter _migrationPlanExecuter;

        public ContentBlocksMigrationsComponent(IMigrationPlanExecuter migrationPlanExecuter)
        {
            _migrationPlanExecuter = migrationPlanExecuter;
        }

        public void Initialize()
        {
            var plan = new ContentBlocksMigrationPlan();
            _migrationPlanExecuter.Execute(plan);
        }

        public void Terminate()
        {
        }
    }
}
