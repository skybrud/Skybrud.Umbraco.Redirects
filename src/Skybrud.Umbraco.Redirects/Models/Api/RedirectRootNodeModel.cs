using System;
using Umbraco.Cms.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models.Api {
    
    public class RedirectRootNodeModel {

        public int Id { get; }

        public Guid Key { get; }

        public string Name { get; }

        public string Icon { get; }

        public string[] Domains { get; }

        public RedirectRootNodeModel(Redirect redirect, IContent content, string[] domains) {
            Id = content?.Id ?? 0;
            Key = content?.Key ?? redirect.RootKey;
            Name = content?.Name;
            Icon = content?.ContentType.Icon;
            Domains = domains ?? Array.Empty<string>();
        }

    }

}