using System.Collections.Generic;

namespace UmbracoAuditViewer.Models
{
    public class Response
    {
        public string NodeName { get; set; }
        public List<Change> Changes { get; set; }
    }
}