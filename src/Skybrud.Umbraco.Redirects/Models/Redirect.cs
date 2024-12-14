using System;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Skybrud.Umbraco.Redirects.Services;

namespace Skybrud.Umbraco.Redirects.Models;

/// <summary>
/// Class representing a redirect.
/// </summary>
public class Redirect : IRedirect {

    private IRedirectDestination _destination;
    private EssentialsTime _createDate;
    private EssentialsTime _updateDate;

    #region Properties

    internal RedirectDto Dto { get; }

    /// <summary>
    /// Gets the ID of the redirect.
    /// </summary>
    public int Id => Dto.Id;

    /// <summary>
    /// Gets the unique ID of the redirect.
    /// </summary>
    public Guid Key => Dto.Key;

    /// <summary>
    /// Gets or sets the root node key of the redirect.
    /// </summary>
    public Guid RootKey {
        get => Dto.RootKey;
        set => Dto.RootKey = value;
    }

    /// <summary>
    /// Gets or sets the inbound path of the redirect. The value will not contain the domain or the query string.
    /// </summary>
    public string Path {
        get => Dto.Path;
        set => Dto.Path = value.TrimEnd('/');
    }

    /// <summary>
    /// Gets or sets the inbound query string of the redirect.
    /// </summary>
    public string? QueryString {
        get => Dto.QueryString;
        set => Dto.QueryString = value;
    }

    /// <summary>
    /// Gets or sets the inbound URL of the redirect.
    /// </summary>
    public string Url {

        get => Dto.Path + (string.IsNullOrWhiteSpace(Dto.QueryString) ? null : "?" + QueryString);

        set {

            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            // Remove the fragment
            value.Split('#', out value);

            // Split the path and query
            value.Split('?', out string path, out string? query);

            // Update the path and query
            Path = path;
            QueryString = query.NullIfWhiteSpace();

        }

    }

    /// <summary>
    /// Gets or sets the destination of the redirect.
    /// </summary>
    public IRedirectDestination Destination {

        get => _destination;

        set {
            _destination = value ?? throw new ArgumentNullException(nameof(value));
            Dto.DestinationId = value.Id;
            Dto.DestinationKey = value.Key;
            Dto.DestinationType = value.Type.ToString();
            Dto.DestinationUrl = value.Url;
            Dto.DestinationQuery = value.Query;
            Dto.DestinationFragment = value.Fragment;
            Dto.DestinationCulture = value.Culture;
        }

    }

    /// <summary>
    /// Gets or sets the timestamp for when the redirect was created.
    /// </summary>
    public EssentialsTime CreateDate {
        get => _createDate;
        set { _createDate = value; Dto.CreateDate = _createDate.DateTimeOffset.ToUniversalTime().DateTime; }
    }

    /// <summary>
    /// Gets or sets the timestamp for when the redirect was last updated.
    /// </summary>
    public EssentialsTime UpdateDate {
        get => _updateDate;
        set { _updateDate = value; Dto.UpdateDate = _updateDate.DateTimeOffset.ToUniversalTime().DateTime; }
    }

    /// <summary>
    /// Gets or sets the type of the redirect. Possible values are <see cref="RedirectType.Permanent"/> and <see cref="RedirectType.Temporary"/>.
    /// </summary>
    public RedirectType Type {
        get => Dto.IsPermanent ? RedirectType.Permanent : RedirectType.Temporary;
        set => Dto.IsPermanent = value == RedirectType.Permanent;
    }

    /// <summary>
    /// Gets or sets whether the redirect is permanent.
    /// </summary>
    public bool IsPermanent {
        get => Dto.IsPermanent;
        set => Dto.IsPermanent = value;
    }

    /// <summary>
    /// Gets or sets whether the query string should be forwarded.
    /// </summary>
    public bool ForwardQueryString {
        get => Dto.ForwardQueryString;
        set => Dto.ForwardQueryString = value;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="dto"/>.
    /// </summary>
    /// <param name="dto">The DTO object received from the database.</param>
    public Redirect(RedirectDto dto) {

        //DateTime createDate = dto.CreateDate.Kind == DateTimeKind.Unspecified ? new DateTime(dto.CreateDate.Ticks) : dto.CreateDate;
        //DateTime updateDate = dto.UpdateDate.Kind == DateTimeKind.Unspecified ? new DateTime(dto.UpdateDate.Ticks) : dto.UpdateDate;

        _createDate = new EssentialsTime(dto.CreateDate.ToLocalTime());
        _updateDate = new EssentialsTime(dto.UpdateDate.ToLocalTime());

        if (!EnumUtils.TryParseEnum(dto.DestinationType, out RedirectDestinationType type)) {
            throw new RedirectsException($"Unknown redirect type: {dto.DestinationType}");
        }

        _destination = new RedirectDestination {
            Type = type,
            //Id = dto.DestinationId,
            Key = dto.DestinationKey,
            Url = dto.DestinationUrl,
            Query = dto.DestinationQuery,
            Fragment = dto.DestinationFragment,
            Culture = dto.DestinationCulture
        };

        Dto = dto;

    }

    #endregion

    #region Static methods

    internal static Redirect CreateFromDto(RedirectDto dto) {
        return new Redirect(dto);
    }

    #endregion

}