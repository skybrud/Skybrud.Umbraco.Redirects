using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Skybrud.Umbraco.Redirects.Notifications {

    public class RoutingRequestNotificationHandler : INotificationHandler<RoutingRequestNotification> {
        
        private readonly IRedirectsService _redirectsService;

        public RoutingRequestNotificationHandler(IRedirectsService redirectsService) {
            _redirectsService = redirectsService;
        }
        
        public void Handle(RoutingRequestNotification notification) {

            if (notification.RequestBuilder.ResponseStatusCode == null) return;
            if (notification.RequestBuilder.ResponseStatusCode.Value != 404) return;

            Redirect redirect = _redirectsService.GetRedirectByUri(notification.RequestBuilder.Uri);
            if (redirect == null) return;

            notification.RequestBuilder.SetRedirect(redirect.Destination.Url);

        }

    }

}