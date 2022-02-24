namespace Skybrud.Umbraco.Redirects.Services {
    
    /// <summary>
    /// Enum class indicating the type of redirects to be returned.
    /// </summary>
    public enum RedirectTypeFilter {
        
        /// <summary>
        /// Indicates that redirects of any type should be returned.
        /// </summary>
        All,

        /// <summary>
        /// Indicates that only content redirects should be returned.
        /// </summary>
        Content,
        
        /// <summary>
        /// Indicates that only media redirects should be returned.
        /// </summary>
        Media,
        
        /// <summary>
        /// Indicates that only URL redirects should be returned.
        /// </summary>
        Url

    }

}