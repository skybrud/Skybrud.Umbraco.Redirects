using Microsoft.Extensions.Options;
using Skybrud.Umbraco.Redirects.Config;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Skybrud.Umbraco.Redirects.Helpers {

    /// <summary>
    /// Class representing the dependencies for the <see cref="RedirectsBackOfficeHelperDependencies"/> class.
    /// </summary>
    public class RedirectsBackOfficeHelperDependencies {

        #region Properties

        /// <summary>
        /// Gets the reference to the current <see cref="GlobalSettings"/>.
        /// </summary>
        public GlobalSettings GlobalSettings { get; }

        /// <summary>
        /// Gets the reference to the current <see cref="IHostingEnvironment"/>.
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the reference to the current <see cref="IRuntimeState"/>.
        /// </summary>
        public IRuntimeState RuntimeState { get; }

        /// <summary>
        /// Gets the reference to the current <see cref="IDomainService"/>.
        /// </summary>
        public IDomainService DomainService { get; }

        /// <summary>
        /// Gets the reference to the current <see cref="IContentService"/>.
        /// </summary>
        public IContentService ContentService { get; }

        /// <summary>
        /// Gets the reference to the current <see cref="IMediaService"/>.
        /// </summary>
        public IMediaService MediaService { get; }

        /// <summary>
        /// Gets the reference to the current <see cref="ILocalizedTextService"/>.
        /// </summary>
        public ILocalizedTextService TextService { get; }

        /// <summary>
        /// Gets a reference to the current <see cref="IBackOfficeSecurityAccessor"/>.
        /// </summary>
        public IBackOfficeSecurityAccessor BackOfficeSecurityAccessor { get; }

        /// <summary>
        /// Gets a reference to the options for the redirects page.
        /// </summary>
        public IOptions<RedirectsSettings> RedirectsSettings { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="runtimeState"></param>
        /// <param name="domainService"></param>
        /// <param name="contentService"></param>
        /// <param name="mediaService"></param>
        /// <param name="textService"></param>
        /// <param name="globalSettings"></param>
        /// <param name="backOfficeSecurityAccessor"></param>
        /// <param name="redirectsSettings"></param>
        public RedirectsBackOfficeHelperDependencies(IOptions<GlobalSettings> globalSettings, IHostingEnvironment hostingEnvironment,
            IRuntimeState runtimeState, IDomainService domainService, IContentService contentService,
            IMediaService mediaService, ILocalizedTextService textService,
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IOptions<RedirectsSettings> redirectsSettings) {
            GlobalSettings = globalSettings.Value;
            HostingEnvironment = hostingEnvironment;
            RuntimeState = runtimeState;
            DomainService = domainService;
            ContentService = contentService;
            MediaService = mediaService;
            TextService = textService;
            BackOfficeSecurityAccessor = backOfficeSecurityAccessor;
            RedirectsSettings = redirectsSettings;
        }

        #endregion

    }

}