using System;
using System.Linq;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Web;

namespace UmbracoAuditViewer.EventHandlers
{
    public class Installer : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            // Update this whenever a migration change is made.
            var targetDbVersion = new SemVersion(1, 0, 0);
            var currentDbVersion = new SemVersion(0, 0, 0);

            var migrations =
                ApplicationContext.Current.Services.MigrationEntryService.GetAll(Constants
                    .ApplicationAlias);
            var latestMigration = migrations
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            if (latestMigration != null)
            {
                currentDbVersion = latestMigration.Version;
            }

            if (targetDbVersion == currentDbVersion)
            {
                return;
            }

            var migrationsRunner = new MigrationRunner(
                ApplicationContext.Current.Services.MigrationEntryService,
                ApplicationContext.Current.ProfilingLogger.Logger,
                currentDbVersion,
                targetDbVersion,
                Constants.ApplicationAlias);

            try
            {
                migrationsRunner.Execute(
                    UmbracoContext.Current.Application.DatabaseContext.Database);
            }
            catch (Exception e)
            {
                LogHelper.Error<Installer>("Error running Umbraco Audit Viewer migrations", e);
            }
        }
    }
}