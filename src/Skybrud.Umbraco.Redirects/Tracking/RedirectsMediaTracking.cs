using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Models.Tracking;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Skybrud.Umbraco.Redirects.Tracking
{
    public class RedirectsMediaTracking : IDataValueReferenceFactory, IDataValueReference
    {
        private readonly IMediaService _mediaService;

        public IDataValueReference GetDataValueReference() => this;

        public RedirectsMediaTracking(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public IEnumerable<UmbracoEntityReference> GetReferences(object value)
        {
            var references = new List<UmbracoEntityReference>();
            if (value != null)
            {
                var redirectLink = JsonConvert.DeserializeObject<RedirectLink>(value.ToString());
                if(redirectLink?.Destination?.Type == "media")
                {
                    AddReferenceFromMediaPath(references, redirectLink?.Destination?.Url);
                }
            }
            return references;
        }

        private void AddReferenceFromMediaPath(List<UmbracoEntityReference> references, string imagePath)
        {
            IMedia media = _mediaService.GetMediaByPath(imagePath);
            if (media == null) return;
            Udi udi = new GuidUdi("media", media.Key);
            references.Add(new UmbracoEntityReference(udi));
        }

        public bool IsForEditor(IDataEditor dataEditor) => dataEditor.Alias.InvariantEquals("Skybrud.Umbraco.Redirects")
            || dataEditor.Alias.InvariantEquals("Skybrud.Umbraco.Redirects.OutboundRedirect");
    }

}