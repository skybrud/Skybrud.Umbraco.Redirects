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
        public void Initialize(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            var plan = new MigrationPlan("Skybrud.Umbraco.Redirects");
            plan.From(string.Empty)
                .To<CreateTable>("0.1.0")
                .To<AddRootNodeIdColumn>("0.2.5")
                .To<AddForwardQueryStringColumn>("0.3.0.a")
                .To<AddIsRegexColumn>("0.3.0");

            var upgrader = new Upgrader(plan);
            upgrader.Execute(scopeProvider, migrationBuilder, keyValueService, logger);
        }

        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }
}
