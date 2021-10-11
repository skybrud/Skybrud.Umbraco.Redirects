using Skybrud.Umbraco.Redirects.PackageConstants;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

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
                foreach (Match image in Regex.Matches(value.ToString(), RedirectsConstants.Tracking.MatchMediaPathPattern))
                {
                    AddReferenceFromMediaPath(references, image);
                }
            }
            return references;
        }

        private void AddReferenceFromMediaPath(List<UmbracoEntityReference> references, Match image)
        {
            IMedia media = _mediaService.GetMediaByPath(image.Value);
            if (media == null) return;
            Udi udi = new GuidUdi("media", media.Key);
            references.Add(new UmbracoEntityReference(udi));
        }

        public bool IsForEditor(IDataEditor dataEditor) =>
            || dataEditor.Alias.InvariantEquals("Skybrud.Umbraco.Redirects")
            || dataEditor.Alias.InvariantEquals("Skybrud.Umbraco.Redirects.OutboundRedirect");
    }
}