using System;
using System.Net.Http;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Exception that wraps any error returned by the BankID API.
    /// </summary>
    public class GrandIdApiException : HttpRequestException
    {
        public GrandIdApiException(string description, Exception inner)
            : this(ErrorCode.Unknown, description, inner)
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
