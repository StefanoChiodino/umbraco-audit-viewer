namespace UmbracoAuditViewer.Models
{
    public class PropertyChange
    {
        public string PropertyName { get; set; }
        public object From { get; set; }
        public object To { get; set; }
    }
}