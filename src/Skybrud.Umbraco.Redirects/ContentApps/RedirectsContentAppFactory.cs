using System.Collections.Generic;
using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.ContentApps {

    public class RedirectsContentAppFactory : IContentAppFactory {

        private readonly RedirectsBackOfficeHelper _backOfficeHelper;

        public RedirectsContentAppFactory(RedirectsBackOfficeHelper backOfficeHelper) {
            _backOfficeHelper = backOfficeHelper;
        }

        public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {
            return _backOfficeHelper.GetContentAppFor(source, userGroups);
        }

    }

}