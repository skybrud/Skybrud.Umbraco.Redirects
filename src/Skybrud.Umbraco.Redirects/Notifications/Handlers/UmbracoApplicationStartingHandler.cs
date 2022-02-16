using Skybrud.Umbraco.Redirects.Migrations;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Skybrud.Umbraco.Redirects.Notifications.Handlers {
    
    public class UmbracoApplicationStartingHandler : INotificationHandler<UmbracoApplicationStartingNotification> {
        
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

        public void Handle(UmbracoApplicationStartingNotification notification) {

            // We don't really want the migration to run before Umbraco is either installed or upgraded, so if the
            // runtime level is less than "Run", we don't create the migration. This is fine as Umbraco will restart
            // after a successful install/upgrade, in which case the runtime level will be "Run" for the next startup
            if (_runtimeState.Level < RuntimeLevel.Run) return;
            
            var plan = new MigrationPlan("Skybrud.Umbraco.Redirects");
            
            plan.From(string.Empty)
                .To<CreateTableMigration>("2.0.0-alpha001")
                .To<RemoveIsRegexColumnMigration>("2.0.0-alpha002")
                .To<AddDestinationColumnsMigration>("2.0.0-alpha008");

            var upgrader = new Upgrader(plan);

            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);

        }

    }

}