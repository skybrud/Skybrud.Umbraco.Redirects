using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Routing;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Redirects.Composers {

    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class RedirectsComposer : IUserComposer {

        public void Compose(Composition composition) {
            composition.Register<IRedirectsService, RedirectsService>(Lifetime.Singleton);
            composition.Register<RedirectsInjectedModule, RedirectsInjectedModule>(); 
        }

    }

}