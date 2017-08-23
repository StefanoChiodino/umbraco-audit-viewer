using umbraco.businesslogic;
using umbraco.interfaces;

namespace UmbracoAuditViewer
{
    [Application("audit", "Audit", "icon-search", sortOrder: 8)]
    public class AuditSection : IApplication { }
}