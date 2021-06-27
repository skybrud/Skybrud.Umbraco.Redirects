using Microsoft.Extensions.Logging;
using Skybrud.Umbraco.Redirects.Migrations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Skybrud.Umbraco.Redirects.Components {
    
    public class MigrationComponent : IComponent {
        
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger<Upgrader> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public MigrationComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger<Upgrader> logger, ILoggerFactory loggerFactory) {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }
        
        public void Initialize() {
            
            var plan = new MigrationPlan("Skybrud.Umbraco.Redirects");
            
            plan.From(string.Empty)
                .To<CreateTableMigration>("2.0.0-alpha001");

            var upgrader = new Upgrader(plan);

            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger, _loggerFactory);

        }

        public void Terminate() { }

    }

}