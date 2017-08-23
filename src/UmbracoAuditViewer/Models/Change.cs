using System;
using System.Collections.Generic;

namespace UmbracoAuditViewer.Models
{
    public class Change
    {
        public string Author { get; set; }
        public DateTime ChangeDateTime { get; set; }
        public List<PropertyChange> PropertyChanges { get; set; }
    }
}