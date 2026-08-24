using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Exceptions;

namespace Skybrud.Umbraco.Redirects.Models.Api;

/// <summary>
/// Class representing a redirects error.
/// </summary>
public class ApiError {

    /// <summary>
    /// Gets the status code of the error.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the message of the error.
    /// </summary>
    public string Error { get; internal set; }

    /// <summary>
    /// Gets the data associated with the error, if any.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="error"/> message.
    /// </summary>
    /// <param name="error">The error message.</param>
    public ApiError(string error) {
        StatusCode = HttpStatusCode.InternalServerError;
        Error = error;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="error"/> message and <paramref name="data"/>.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="data">The data associated with the error.</param>
    public ApiError(string error, object? data) {
        StatusCode = HttpStatusCode.InternalServerError;
        Error = error;
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public ApiError(RedirectsException exception) {
        StatusCode = exception.StatusCode;
        Error = exception.Message;
    }

}