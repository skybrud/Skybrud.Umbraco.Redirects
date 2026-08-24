using Umbraco.Cms.Api.Management.OpenApi;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Api;

public class RedirectsSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    protected override string ApiName => RedirectsApiConstants.Alias;

}