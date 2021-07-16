using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Core.Dashboards;

namespace Skybrud.Umbraco.Redirects.Dashboards {
    
    public class RedirectsDashboard : IDashboard {
        
        private readonly RedirectsBackOfficeHelper _backoffice;
        
        public string Alias => "Skybrud.Umbraco.Redirects";

        public string[] Sections => new[] { "content" };

        public string View => $"/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dashboard.html?v={_backoffice.GetCacheBuster()}";

        public IAccessRule[] AccessRules => _backoffice.GetDashboardAccessRules();

        public RedirectsDashboard(RedirectsBackOfficeHelper backoffice) {
            _backoffice = backoffice;
        }

    }

}