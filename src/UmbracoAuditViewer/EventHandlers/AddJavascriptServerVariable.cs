using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;
using UmbracoAuditViewer.Controllers;

namespace UmbracoAuditViewer.EventHandlers
{
    public class AddJavascriptServerVariable : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            ServerVariablesParser.Parsing += AddAuditControllerBaseUrlToServerVariables;
        }

        /// <summary>
        /// Adds the route to the <see cref="AuditController"/> for the
        /// Angular controller to find.
        /// </summary>
        private static void AddAuditControllerBaseUrlToServerVariables(
            object sender,
            Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            var urlHelper = new UrlHelper(
                new RequestContext(
                    new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var mainDictionaryProviders = new Dictionary<string, object>();
            var umbracoApiServiceBaseUrl = urlHelper
                .GetUmbracoApiServiceBaseUrl<AuditController>(
                    controller => controller.GetChanges(0));
            mainDictionaryProviders.Add("BaseUrl", umbracoApiServiceBaseUrl);

            if (!e.Keys.Contains("AuditController"))
            {
                e.Add("AuditController", mainDictionaryProviders);
            }
        }
    }
}