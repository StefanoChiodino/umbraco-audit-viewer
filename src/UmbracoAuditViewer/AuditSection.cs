using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Web.Mvc;

namespace UmbracoAuditViewer
{
    [Application("audit", "audit", "Audit", sortOrder: 8)]
    [PluginController("Audit")]
    public class AuditSection : IApplication { }
}