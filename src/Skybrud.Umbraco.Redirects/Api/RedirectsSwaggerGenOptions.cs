using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Skybrud.Umbraco.Redirects.Api.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Api;

public class RedirectsSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {

    public void Configure(SwaggerGenOptions options) {

        options.SwaggerDoc(RedirectsApiConstants.Alias, new OpenApiInfo {
            Title = RedirectsApiConstants.Name,
            Version = RedirectsApiConstants.Version
        });

        options.OperationFilter<RedirectsOperationFilter>();
        options.DocumentFilter<RedirectsDocumentFilter>();
        options.SchemaFilter<RedirectsSchemaFilter>();

    }

}