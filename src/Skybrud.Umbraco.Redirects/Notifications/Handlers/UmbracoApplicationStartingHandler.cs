using System.Threading;
using System.Threading.Tasks;
using Skybrud.Umbraco.Redirects.Migrations;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Notifications.Handlers;

public class UmbracoApplicationStartingHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification> {

    private readonly IScopeProvider _scopeProvider;
    private readonly IMigrationPlanExecutor _migrationPlanExecutor;
    private readonly IKeyValueService _keyValueService;
    private readonly IRuntimeState _runtimeState;

    public UmbracoApplicationStartingHandler(IScopeProvider scopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState) {
        _scopeProvider = scopeProvider;
        _migrationPlanExecutor = migrationPlanExecutor;
        _keyValueService = keyValueService;
        _runtimeState = runtimeState;
    }

    public Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken) {

        // We don't really want the migration to run before Umbraco is either installed or upgraded, so if the
        // runtime level is less than "Run", we don't create the migration. This is fine as Umbraco will restart
        // after a successful install/upgrade, in which case the runtime level will be "Run" for the next startup
        if (_runtimeState.Level < RuntimeLevel.Run) return Task.CompletedTask;

        var plan = new MigrationPlan(RedirectsPackage.Alias);

        plan.From(string.Empty)
            .To<CreateTableMigration>("2.0.0-alpha001")
            .To<RemoveIsRegexColumnMigration>("2.0.0-alpha002")
            .To<DummyMigration>("2.0.0-alpha008")
            .To<FixRootKeyValue>("2.0.5")
            .To<DummyMigration>("2.1.1")
            .To<AddDestinationColumnsMigration>("3.0.0-alpha008")
            .To<RemoveRootIdColumnMigration>("4.0.4")
            .To<AddDestinationCultureColumnMigration>("4.0.9")
            .To<DummyMigration>("8934735b")
            .To<AddDestinationNameColumnMigration>("c0d4ddeb")
            .To<NullableColumnsMigration>("4b5fabfc");

        var upgrader = new Upgrader(plan);

        upgrader.ExecuteAsync(_migrationPlanExecutor, _scopeProvider, _keyValueService);

        return Task.CompletedTask;

    }

}