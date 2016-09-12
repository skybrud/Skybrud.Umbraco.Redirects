using Skybrud.Umbraco.Redirects.Routing;
using Umbraco.Core;
using Umbraco.Web.Routing;

namespace Skybrud.Umbraco.Redirects {

    public class Startup : IApplicationEventHandler {
        
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) { }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
            //ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByNotFoundHandlers, RedirectsContentFinder>();
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)  { }

    }

}