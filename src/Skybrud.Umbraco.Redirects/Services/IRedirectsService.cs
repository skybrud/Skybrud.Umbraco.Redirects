using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Services {
    
    public interface IRedirectsService {

        Redirect GetRedirectByUrl(int rootNodeId, string url);

        Redirect GetRedirectByRequest(HttpRequest request);

        Redirect GetRedirectByUri(Uri uri);

        /// <summary>
        /// Returns the calculated destionation URL of a redirect.
        ///
        /// For redirects poiting to a content or media item within Umbraco, the method will attempt to find the
        /// current URL of that item - eg. if the URL was changed since the redirect was created.
        ///
        /// If enabled for the redirect, this method will also handle query string forwarding as well as other related scenarios.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <returns>The destination URL.</returns>
        string GetDestinationUrl(Redirect redirect);

        public RedirectsSearchResult GetRedirects(int page = 1, int limit = 20, string type = null, string text = null, int? rootNodeId = null);

        /// <summary>
        /// Returns a collection with all redirects.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{Redirect}"/>.</returns>
        public IEnumerable<Redirect> GetAllRedirects();

    }

}