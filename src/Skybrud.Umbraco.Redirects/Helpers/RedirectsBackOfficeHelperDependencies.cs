using Umbraco.Cms.Core.Services;

namespace Skybrud.Umbraco.Redirects.Helpers {
    
    /// <summary>
    /// Class representing the dependencies for the <see cref="RedirectsBackOfficeHelperDependencies"/> class.
    /// </summary>
    public class RedirectsBackOfficeHelperDependencies {

        #region Properties
        
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        /// <param name="runtimeState"></param>
        /// <param name="domainService"></param>
        /// <param name="contentService"></param>
        /// <param name="mediaService"></param>
        /// <param name="textService"></param>
        public RedirectsBackOfficeHelperDependencies(IRuntimeState runtimeState, IDomainService domainService, IContentService contentService,
            IMediaService mediaService, ILocalizedTextService textService) {
            RuntimeState = runtimeState;
            DomainService = domainService;
            ContentService = contentService;
            MediaService = mediaService;
            TextService = textService;
        }

        #endregion

    }

}