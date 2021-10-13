using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.Models.Tracking;
using System;
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
                var redirectLink = JsonConvert.DeserializeObject<OutboundRedirect>(value.ToString());
                if(redirectLink?.Destination?.Type == Models.RedirectDestinationType.Media)
                {
                    AddReferenceFromMediaPath(references, redirectLink?.Destination?.Url);
                }
                else if (redirectLink?.Destination?.Type == Models.RedirectDestinationType.Content)
                {
                    AddReferenceToContent(references, redirectLink?.Destination?.Key);
                }
            }
            return references;
        }
        private void AddReferenceToContent(List<UmbracoEntityReference> references, Guid? key)
        {
            if (key == null) return;
            Udi udi = new GuidUdi("content", key.Value);
            references.Add(new UmbracoEntityReference(udi));
        }

        private void AddReferenceFromMediaPath(List<UmbracoEntityReference> references, string imagePath)
        {
            IMedia media = _mediaService.GetMediaByPath(imagePath);
            if (media == null) return;
            Udi udi = new GuidUdi("media", media.Key);
            references.Add(new UmbracoEntityReference(udi));
        }

        public bool IsForEditor(IDataEditor dataEditor) => dataEditor.Alias.InvariantEquals("Skybrud.Umbraco.Redirects.OutboundRedirect");
    }

}