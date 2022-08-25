using System.Net.Http;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api;

/// <summary>
/// Exception that wraps any error returned by the GrandID API.
/// </summary>
public class GrandIdApiException : HttpRequestException
{
    internal GrandIdApiException(Error error)
        : this(error.GetErrorCode(), error.Message)
    { }

    internal GrandIdApiException(ErrorCode errorCode, string errorDetails)
        : base($"{errorCode}: {errorDetails}", null)
    {
        ErrorCode = errorCode;
        ErrorDetails = errorDetails;
    }

    /// <summary>
    /// The category of error.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <summary>
    /// Details about the error.
    /// </summary>
    public string ErrorDetails { get; }
}