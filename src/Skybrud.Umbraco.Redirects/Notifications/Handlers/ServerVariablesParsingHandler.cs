using System.Collections.Generic;
using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Skybrud.Umbraco.Redirects.Notifications.Handlers {
    
    public class ServerVariablesParsingHandler : INotificationHandler<ServerVariablesParsingNotification> {
        
        private readonly RedirectsBackOfficeHelper _backoffice;

        public ServerVariablesParsingHandler(RedirectsBackOfficeHelper backoffice) {
            _backoffice = backoffice;
        }
        
        public void Handle(ServerVariablesParsingNotification notification) {
            
            // Get or create the "skybrud" dictionary
            if (!(notification.ServerVariables.TryGetValue("skybrud", out object value) && value is Dictionary<string, object> skybrud))  {
                notification.ServerVariables["skybrud"] = skybrud = new Dictionary<string, object>();
            }

            // Append the "redirects" dictionary to "skybrud"
            skybrud.Add("redirects", _backoffice.GetServerVariables());

        }

    }

}