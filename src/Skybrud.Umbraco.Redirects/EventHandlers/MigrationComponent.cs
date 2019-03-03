using Skybrud.Umbraco.Redirects.Migrations;
using Umbraco.Core.Composing;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Logging;

namespace Skybrud.Umbraco.Redirects.EventHandlers
{
    public class MigrationComponent : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger _logger;

        public MigrationComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }
        public void Initialize()
        {
            var plan = new MigrationPlan("Skybrud.Umbraco.Redirects");
            plan.From(string.Empty)
                .To<CreateTable>("0.1.0")
                .To<AddRootNodeIdColumn>("0.2.5")
                .To<AddForwardQueryStringColumn>("0.3.0.a")
                .To<AddIsRegexColumn>("0.3.0");

            var upgrader = new Upgrader(plan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
}
