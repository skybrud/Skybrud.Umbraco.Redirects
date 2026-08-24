using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Api;

public class RedirectsSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {

    public void Configure(SwaggerGenOptions options) {
        options.SwaggerDoc(RedirectsApiConstants.Alias, new OpenApiInfo {
            Title = RedirectsApiConstants.Name,
            Version = "1.0"
        });
        options.OperationFilter<RedirectsSecurityFilter>();
    }

}