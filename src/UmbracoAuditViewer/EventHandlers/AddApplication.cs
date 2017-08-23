using Umbraco.Core;

namespace UmbracoAuditViewer.EventHandlers
{
    public class AddApplication : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            applicationContext.Services.SectionService.MakeNew(
                Constants.ApplicationName, Constants.ApplicationAlias, Constants.ApplicationIcon);
        }
    }
}