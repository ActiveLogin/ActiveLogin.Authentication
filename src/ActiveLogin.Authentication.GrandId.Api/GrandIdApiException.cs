using System;
using System.Net.Http;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Exception that wraps any error returned by the GrandID API.
    /// </summary>
    public class GrandIdApiException : HttpRequestException
    {
        public GrandIdApiException(string description, Exception inner)
            : this(ErrorCode.UNKNOWN, description, inner)
        { }

        public GrandIdApiException(Error error, Exception inner)
            : this(error.ErrorCode, error.Details, inner)
        { }

        public GrandIdApiException(ErrorCode errorCode, string details)
            : this(errorCode, details, null)
        { }

        public GrandIdApiException(ErrorCode errorCode, string details, Exception inner)
            : base($"{errorCode}: {details}", inner)
        {
            ErrorCode = errorCode;
            Details = details;
        }

        public GrandIdApiException(string errorCodeString, string details)
          : base($"{errorCodeString}: {details}", null)
        {
            ErrorCode errorCode;
            ErrorCode = Enum.TryParse<ErrorCode>(errorCodeString, out errorCode) ? errorCode : ErrorCode.UNKNOWN;
            Details = details;
        }


        /// <summary>
        /// The category of error.
        /// </summary>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        /// Details about the error.
        /// </summary>
        public string Details { get; }
    }
}
