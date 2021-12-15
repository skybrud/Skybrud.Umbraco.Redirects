using System;
using Umbraco.Cms.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models.Api {
    
    public class RedirectRootNodeModel {

        public int Id { get; }

        public Guid Key { get; }

        public string Name { get; }

        public string Icon { get; }

        public string BackOfficeUrl => $"/umbraco/#/content/content/edit/{Id}";

        public string[] Domains { get; }

        public RedirectRootNodeModel(IRedirect redirect, IContent content, string[] domains) {
            Id = content?.Id ?? 0;
            Key = content?.Key ?? redirect.RootKey;
            Name = content?.Name;
            Icon = content?.ContentType.Icon;
            Domains = domains ?? Array.Empty<string>();
        }

        public RedirectRootNodeModel(RedirectRootNode rootNode) {
            Id = rootNode.Id;
            Key = rootNode.Key;
            Name = rootNode.Name;
            Icon = rootNode.Icon;
            Domains = rootNode.Domains;
        }

    }

}