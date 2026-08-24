using Microsoft.OpenApi;
using Skybrud.Essentials.Time;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Skybrud.Umbraco.Redirects.Api.Filters;

#pragma warning disable CS1591

/// <remarks>
/// This schema filter is used to customize the OpenAPI schema generation for the <see cref="EssentialsTime"/> type. It
/// ensures that the schema is represented as a string with a date-time format, and removes any unnecessary object-like
/// schema information that Swagger may have generated. Changes only applies to models within this package.
/// </remarks>
internal sealed class RedirectsSchemaFilter : ISchemaFilter {

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context) {

        if (context.MemberInfo is null) return;
        if (schema is not OpenApiSchema openApiSchema) return;
        if (context.Type != typeof(EssentialsTime)) return;
        if (context.MemberInfo?.DeclaringType?.Assembly != typeof(RedirectsSchemaFilter).Assembly) return;

        openApiSchema.Type = JsonSchemaType.String;
        openApiSchema.Format = "date-time";

        // Remove any object-like schema information Swagger may have generated.
        openApiSchema.Properties?.Clear();
        openApiSchema.Required?.Clear();
        openApiSchema.Items = null;
        openApiSchema.OneOf?.Clear();
        openApiSchema.DynamicRef = null;
        openApiSchema.AdditionalProperties = null;

    }

}