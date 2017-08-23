using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace UmbracoAuditViewer.Migrations
{
    [Migration("1.0.0", 1, Constants.ApplicationAlias)]
    public class AddSummaryDashboardToConfig : MigrationBase
    {
        private readonly addDashboardSection _packageAction;
        private readonly XmlNode _xml;

        public AddSummaryDashboardToConfig(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            _packageAction = new addDashboardSection();

            const string xml = @"
  <section alias=""Audit"">
    <areas>
      <area>
        audit
    </area>
    </areas>
    <tab caption=""Audit"">
      <control showOnce=""true"" addPanel=""true"" panelCaption="""">
        /app_plugins/Audit/Audit.html
      </control>
    </tab>
  </section>";

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            _xml = xdoc.DocumentElement;
        }

        public override void Up()
        {
            _packageAction.Execute(Constants.ApplicationAlias, _xml);
        }

        public override void Down()
        {
            _packageAction.Undo(Constants.ApplicationAlias, _xml);
        }
    }
}
