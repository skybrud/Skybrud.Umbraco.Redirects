using Skybrud.Umbraco.Redirects.Components;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Routing;
using Skybrud.Umbraco.Redirects.Tracking;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Redirects.Composers {

    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class RedirectsComposer : IUserComposer {

        public void Compose(Composition composition) {
            composition.Components().Append<RedirectsComponent>();
            composition.Register<IRedirectsService, RedirectsService>(Lifetime.Singleton);
            composition.Register<RedirectsInjectedModule, RedirectsInjectedModule>();
            composition.Register<RedirectsBackOfficeHelper>();
            composition.DataValueReferenceFactories().Append<RedirectsMediaTracking>();
        }

    }

}