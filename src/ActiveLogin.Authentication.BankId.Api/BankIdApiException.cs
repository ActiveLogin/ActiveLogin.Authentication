using System;
using System.Net.Http;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    /// <summary>
    ///     Exception that wraps any error returned by the BankID API.
    /// </summary>
    public class BankIdApiException : HttpRequestException
    {
        internal BankIdApiException(Error error, Exception inner)
            : this(error.GetErrorCode(), error.Details, inner)
        {
        }

        internal BankIdApiException(ErrorCode errorCode, string errorDetails)
            : this(errorCode, errorDetails, null)
        {
        }

        internal BankIdApiException(ErrorCode errorCode, string errorDetails, Exception inner)
            : base($"{errorCode}: {errorDetails}", inner)
        {
            ErrorCode = errorCode;
            ErrorDetails = errorDetails;
        }

        /// <summary>
        ///     The category of error.
        /// </summary>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        ///     Details about the error.
        /// </summary>
        public string ErrorDetails { get; }
    }
}
