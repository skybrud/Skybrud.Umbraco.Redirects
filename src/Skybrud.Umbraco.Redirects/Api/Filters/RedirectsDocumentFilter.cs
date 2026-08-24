using System;
using Microsoft.OpenApi;
using Skybrud.Essentials.Time;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Skybrud.Umbraco.Redirects.Api.Filters;

#pragma warning disable CS1591

/// <summary>
/// This document filter is used to remove the <see cref="EssentialsTime"/> schema from the OpenAPI document for the
/// Skybrud Redirects API. This is necessary because the <see cref="EssentialsTime"/> type is represented as a string with a
/// date-time format, and we want to avoid including unnecessary object-like schema information in the generated
/// OpenAPI documentation. The filter only applies to the Skybrud Redirects API document.
/// </summary>
internal class RedirectsDocumentFilter : IDocumentFilter {

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
        if (context.DocumentName != RedirectsApiConstants.Alias) return;
        swaggerDoc.Components?.Schemas?.Remove(nameof(EssentialsTime));
        swaggerDoc.Components?.Schemas?.Remove(nameof(DayOfWeek));
        swaggerDoc.Components?.Schemas?.Remove(nameof(TimeZoneInfo));
    }

}