using Skybrud.Umbraco.Redirects.Migrations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Skybrud.Umbraco.Redirects.Components {
    
    public class MigrationComponent : IComponent {
        
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IScopeProvider _scopeProvider;
        private readonly IKeyValueService _keyValueService;

        public MigrationComponent(IMigrationPlanExecutor migrationPlanExecutor, IScopeProvider scopeProvider, IKeyValueService keyValueService) {
            _scopeProvider = scopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
        }
        
        public void Initialize() {
            
            var plan = new MigrationPlan("Skybrud.Umbraco.Redirects");
            
            plan.From(string.Empty)
                .To<CreateTableMigration>("2.0.0-alpha001");

            var upgrader = new Upgrader(plan);

            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);

        }

        public void Terminate() { }

    }

}