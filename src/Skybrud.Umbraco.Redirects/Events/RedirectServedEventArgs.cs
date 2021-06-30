using Skybrud.Umbraco.Redirects.Models;
using System;

namespace Skybrud.Umbraco.Redirects.Events
{
    public class RedirectServedEventArgs : EventArgs
    {
        public RedirectItem Redirect { get; set; }
    }
}
