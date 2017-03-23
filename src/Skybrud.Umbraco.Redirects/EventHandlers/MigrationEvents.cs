using System;
using System.Collections.Generic;
using System.Linq;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Web;

namespace Skybrud.Umbraco.Redirects.EventHandlers {

    public class MigrationEvents : ApplicationEventHandler {
        
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
            HandleStatisticsMigration();
        }

        private static void HandleStatisticsMigration() {

            // Declare an initial version (will be used as fallback fro new install)
            SemVersion currentVersion = new SemVersion(0);

            // Get all migrations for "Skybrud.Redirects" already executed
            IEnumerable<IMigrationEntry> migrations = ApplicationContext.Current.Services.MigrationEntryService.GetAll(Package.Alias);

            // Get the latest migration for "Skybrud.Redirects" executed
            IMigrationEntry latestMigration = migrations.OrderByDescending(x => x.Version).FirstOrDefault();

            if (latestMigration != null) currentVersion = latestMigration.Version;

            // Get the target version (and return if already up-to-date)
            SemVersion targetVersion = Package.SemVersion;
            if (targetVersion == currentVersion) return;

            // Initialize a new migration runner for our package
            MigrationRunner migrationsRunner = new MigrationRunner(
                ApplicationContext.Current.Services.MigrationEntryService,
                ApplicationContext.Current.ProfilingLogger.Logger,
                currentVersion,
                targetVersion,
                Package.Alias
            );

            try {
                migrationsRunner.Execute(UmbracoContext.Current.Application.DatabaseContext.Database);
            } catch (Exception e) {
                LogHelper.Error<MigrationEvents>("Error running " + Package.Alias + " migration", e);
            }

        }

    }

}