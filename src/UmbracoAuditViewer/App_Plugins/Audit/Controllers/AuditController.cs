using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Editors;

namespace UmbracoAuditViewer.App_Plugins.Audit.Controllers
{
    public class AuditController : UmbracoAuthorizedJsonController
    {
        public class Response
        {
            public string NodeName { get; set; }
            public List<Change> Changes { get; set; }
        }

        public class Change
        {
            public string Author { get; set; }
            public DateTime ChangeDateTime { get; set; }
            public List<PropertyChange> PropertyChanges { get; set; }
        }

        public class PropertyChange
        {
            public string PropertyName { get; set; }
            public object From { get; set; }
            public object To { get; set; }
        }

        [HttpGet]
        public Response GetChanges(int id)
        {
            var versions = ApplicationContext.Services.ContentService
                .GetVersions(id)
                .ToList();

            var publishedVersion = versions
                .FirstOrDefault(v => v.Published);

            int usersTotalRecods;
            // As there is no way to get a subset of the users I hope that eager
            // loading them will be quicker than lazy loading specific ones, one at a time.
            var users = ApplicationContext.Services.UserService
                .GetAll(0, int.MaxValue, out usersTotalRecods);

            var changes = GetChanges(versions, users);

            var response = new Response
                {
                    NodeName = publishedVersion?.Name,
                    Changes = changes
                };

            return response;
        }

        private static List<Change> GetChanges(
            IReadOnlyCollection<IContent> versions,
            IEnumerable<IUser> users)
        {
            var changes = versions
                .Select((v, i) =>
                {
                    var previousVersion = versions.ElementAtOrDefault(i - 1);
                    var propertyChanges = GetPropertyChanges(v, previousVersion);

                    var change = new Change
                        {
                            Author = users
                                .FirstOrDefault(u => u.Id == v.WriterId)
                                ?.Name,
                            ChangeDateTime = v.UpdateDate,
                            PropertyChanges = propertyChanges
                        };

                    return change;
                })
                .ToList();

            return changes;
        }

        private static List<PropertyChange> GetPropertyChanges(
            IContent version,
            IContent previousVersion)
        {
            var addedPropertyChanges = GetAddedPropertyChanges(version, previousVersion);
            var removedPropertiesChanges = GetRemovedPropertiesChanges(version, previousVersion);

            // Now get all the properties that have not been added or removed and
            // compare the values.
            var propertyVersionPair = version
                .Properties
                .Select(p => new
                    {
                        Property = p,
                        PreviousProperty = previousVersion?
                            .Properties
                            .FirstOrDefault(pp => pp.PropertyType.Id == p.PropertyType.Id),
                    });
            var propertyPairWithHistory = propertyVersionPair
                .Where(p => p.PreviousProperty != null);
            
            var propertyPairThatDiffer = propertyPairWithHistory
                .Where(p => JsonConvert.SerializeObject(p.PreviousProperty.Value) !=
                    JsonConvert.SerializeObject(p.Property.Value));
            var changes = propertyPairThatDiffer
                .Select(p => new PropertyChange
                    {
                        PropertyName = p.Property.PropertyType.Name,
                        From = p.PreviousProperty.Value,
                        To = p.Property.Value,
                    })
                .ToList();

            var propertyChanges = addedPropertyChanges
                .Concat(removedPropertiesChanges)
                .Concat(changes)
                .ToList();

            return propertyChanges;
        }

        /// <summary>
        /// Proprieties that have been removed from the previous version to the current.
        /// </summary>
        private static List<PropertyChange> GetRemovedPropertiesChanges(
            IContent version,
            IContent previousVersion)
        {
            var removedProperties = previousVersion?.Properties
                .Where(pp => version.Properties
                    .All(p => p.PropertyType.Id != pp.PropertyType.Id))
                .ToList()
                ?? Enumerable.Empty<Property>();

            var propertyChanges = removedProperties
                .Select(ap => new PropertyChange
                    {
                        PropertyName = ap.PropertyType.Name,
                        From = ap.Value,
                        To = null,
                    })
                .ToList();

            return propertyChanges;
        }

        /// <summary>
        /// Properties that have been added from the previous version to the current.
        /// </summary>
        private static List<PropertyChange> GetAddedPropertyChanges(
            IContent version,
            IContent previousVersion)
        {
            var addedProprieties = previousVersion == null
                ? version.Properties
                    .Where(p => p.Value != null)
                    .ToList()
                : version.Properties
                    .Where(p => previousVersion.Properties
                        .All(pp => pp.PropertyType.Id != p.PropertyType.Id))
                    .ToList();
            var propertyChanges = addedProprieties
                .Select(ap => new PropertyChange
                    {
                        PropertyName = ap.PropertyType.Name,
                        From = null,
                        To = ap.Value,
                    })
                .ToList();

            return propertyChanges;
        }
    }
}
