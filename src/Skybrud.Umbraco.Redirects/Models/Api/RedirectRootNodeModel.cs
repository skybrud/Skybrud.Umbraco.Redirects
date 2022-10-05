using System;
using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.Models;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectRootNodeModel {

        public int Id { get; }

        public Guid Key { get; }

        public string Name { get; }

        public string Icon { get; }

        public string BackOfficeUrl { get; }

        public string[] Domains { get; }

        public RedirectRootNodeModel(IRedirect redirect, IContent content, string[] domains, string backOfficeBaseUrl) {
            Id = content?.Id ?? 0;
            Key = content?.Key ?? redirect.RootKey;
            Name = content?.Name;
            Icon = content?.ContentType.Icon;
            Domains = domains ?? Array.Empty<string>();
            BackOfficeUrl = $"{backOfficeBaseUrl}/#/content/content/edit/{Id}";
        }

        public RedirectRootNodeModel(IRedirect redirect, IContent content, string[] domains, RedirectsBackOfficeHelper backOffice) {
            Id = content?.Id ?? 0;
            Key = content?.Key ?? redirect.RootKey;
            Name = content?.Name;
            Icon = content?.ContentType.Icon;
            Domains = domains ?? Array.Empty<string>();
            BackOfficeUrl = $"{backOffice.BackOfficeUrl}/#/content/content/edit/{Id}";
        }

        public RedirectRootNodeModel(RedirectRootNode rootNode, string backOfficeBaseUrl) {
            Id = rootNode.Id;
            Key = rootNode.Key;
            Name = rootNode.Name;
            Icon = rootNode.Icon;
            Domains = rootNode.Domains;
            BackOfficeUrl = $"{backOfficeBaseUrl}/#/content/content/edit/{Id}";
        }

        public RedirectRootNodeModel(RedirectRootNode rootNode, RedirectsBackOfficeHelper backOffice) {
            Id = rootNode.Id;
            Key = rootNode.Key;
            Name = rootNode.Name;
            Icon = rootNode.Icon;
            Domains = rootNode.Domains;
            BackOfficeUrl = $"{backOffice.BackOfficeUrl}/#/content/content/edit/{Id}";
        }

    }

}