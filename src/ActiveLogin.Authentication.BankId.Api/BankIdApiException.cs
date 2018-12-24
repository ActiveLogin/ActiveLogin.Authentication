using System;
using System.Net.Http;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    /// <summary>
    /// Exception that wraps any error returned by the BankID API.
    /// </summary>
    public class BankIdApiException : HttpRequestException
    {
        internal BankIdApiException(Error error)
            : this(error, null)
        { }

        internal BankIdApiException(Error error, Exception inner)
            : this(error.ErrorCode, error.Details, inner)
        { }

        private BankIdApiException(string errorCodeString, string details, Exception inner)
            : base($"{errorCodeString}: {details}", inner)
        {
            ErrorCode = Enum.TryParse<ErrorCode>(errorCodeString, true, out var errorCode) ? errorCode : ErrorCode.Unknown;
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
