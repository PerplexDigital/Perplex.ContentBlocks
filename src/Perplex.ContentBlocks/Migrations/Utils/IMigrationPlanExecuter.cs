using Umbraco.Core.Migrations;

namespace Perplex.ContentBlocks.Migrations.Utils
{
    public interface IMigrationPlanExecuter
    {
        void Execute(MigrationPlan plan);
    }
}
