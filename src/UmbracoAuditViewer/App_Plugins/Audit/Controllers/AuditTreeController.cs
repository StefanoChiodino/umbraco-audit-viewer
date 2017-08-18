using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace UmbracoAuditViewer.App_Plugins.Audit.Controllers
{
    [Tree("audit", "audit", "Audit", sortOrder: 0)]
    [PluginController("Audit")]
    public class AuditTreeController : TreeController
    {
        private readonly ImmutableList<int> _configuredRootNodeIds = GetAppSettingsRootNodeIds();

        /// <summary>
        /// Gets the node ids configured in the app settings.
        /// </summary>
        private static ImmutableList<int> GetAppSettingsRootNodeIds()
        {
            var setting = ConfigurationManager
                .AppSettings["Audit:RootNodeIds"];
            var rootNodes = CommaSeparatedStringNumbersToInts(setting)
                .ToImmutableList();

            return rootNodes;
        }

        /// <summary>
        /// Get all the tree nodes under the the node with
        /// <param name="id">the specified id</param>.
        /// </summary>
        /// <param name="id">The parent id</param>
        protected override TreeNodeCollection GetTreeNodes(string id,
            FormDataCollection queryStrings)
        {
            int idInt;
            if (!int.TryParse(id, out idInt))
            {
                throw new ArgumentOutOfRangeException($"id \"{id}\" needs to be a number!");
            }

            // -1 is by convention the root.
            ImmutableList<IPublishedContent> nodes;
            if (idInt == -1)
            {
                nodes = _configuredRootNodeIds
                    .Select(nodeId => UmbracoContext.ContentCache.GetById(nodeId))
                    .ToImmutableList();
                if (!nodes.Any())
                {
                    nodes = UmbracoContext.ContentCache.GetAtRoot()
                        .ToImmutableList();
                }
            }
            else
            {
                nodes = UmbracoContext.ContentCache
                    .GetByXPath($"//*[@id=\"{id}\" and @isDoc]/*[@isDoc]")
                    .ToImmutableList();
            }

            var treeNodeCollection = new TreeNodeCollection();
            var treeNodes = nodes
                .Select(GetTreeNode)
                .ToList();
            treeNodeCollection.AddRange(treeNodes);

            return treeNodeCollection;
        }

        private TreeNode GetTreeNode(IPublishedContent publishedContent)
        {
            var contentType =
                Services.ContentTypeService.GetContentType(publishedContent.ContentType.Id);
            
            var nodeTree = CreateTreeNode(
                publishedContent.Id.ToString(),
                publishedContent.Parent?.ToString(),
                null,
                publishedContent.Name,
                contentType.Icon,
                publishedContent.Children.Any(),
                $"audit/audit/view/{publishedContent.Id}");

            return nodeTree;
        }

        protected override MenuItemCollection GetMenuForNode(string id,
            FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        private static IEnumerable<int> CommaSeparatedStringNumbersToInts(string commaSeparatedNumbers)
        {
            if (commaSeparatedNumbers.IsNullOrWhiteSpace())
            {
                yield break;
            }

            var numberStrings = commaSeparatedNumbers.Split(',');
            foreach (var numberString in numberStrings)
            {
                int number;
                if (int.TryParse(numberString, out number))
                {
                    yield return number;
                }
            }
        }
    }
}