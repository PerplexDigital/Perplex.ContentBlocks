using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Perplex.ContentBlocks.Migrations;

public class ContentBlocksMigrationPlanComponent(
    IMigrationPlanExecutor migrationPlanExecutor,
    ICoreScopeProvider coreScopeProvider,
    IKeyValueService keyValueService,
    ILogger<ContentBlocksMigrationPlanComponent> logger)
    : INotificationAsyncHandler<RuntimePremigrationsUpgradeNotification>
{
    public async Task HandleAsync(RuntimePremigrationsUpgradeNotification notification, CancellationToken cancellationToken)
    {
        if (notification.UpgradeResult == RuntimePremigrationsUpgradeNotification.PremigrationUpgradeResult.HasErrors)
        {
            logger.LogWarning("Skipping Perplex.ContentBlocks migrations due errors in the Umbraco premigration step.");
            return;
        }

        var plan = new ContentBlocksMigrationPlan();
        var upgrader = new Upgrader(plan);
        await upgrader.ExecuteAsync(migrationPlanExecutor, coreScopeProvider, keyValueService);
    }
}

public class ContentBlocksMigrationPlanComponentComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<RuntimePremigrationsUpgradeNotification, ContentBlocksMigrationPlanComponent>();
    }
}
