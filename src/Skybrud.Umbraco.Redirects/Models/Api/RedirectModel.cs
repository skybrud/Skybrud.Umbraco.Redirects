using System;
using System.Text.Json.Serialization;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Text.Json;

namespace Skybrud.Umbraco.Redirects.Models.Api {
    
    public class RedirectModel {

        private readonly Redirect _redirect;
        
        public int Id => _redirect.Id;
        
        public Guid Key => _redirect.Key;

        public RedirectRootNodeModel RootNode { get; }
        
        public string Path => _redirect.Path;
        
        public string QueryString => _redirect.QueryString;
        
        public string Url => _redirect.Url;

        public RedirectDestinationModel Destination { get; }

        [JsonConverter(typeof(Iso8601TimeConverter))]
        public EssentialsTime CreateDate => _redirect.CreateDate;

        [JsonConverter(typeof(Iso8601TimeConverter))]
        public EssentialsTime UpdateDate => _redirect.UpdateDate;
        
        public RedirectType Type => _redirect.Type;
        
        [JsonPropertyName("permanent")]
        public bool IsPermanent => _redirect.IsPermanent;

        [JsonPropertyName("forward")]
        public bool ForwardQueryString => _redirect.ForwardQueryString;

        public RedirectModel(Redirect redirect, RedirectRootNodeModel rootNode, RedirectDestinationModel destination) {
            _redirect = redirect;
            RootNode = rootNode;
            Destination = destination;
        }

    }

}