using System;
using System.Net.Http;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    public class BankIdApiException : HttpRequestException
    {
        public BankIdApiException(string description, Exception inner)
            : this(ErrorCode.Unknown, description, inner)
        { }

        public BankIdApiException(Error error, Exception inner)
            : this(error.ErrorCode, error.Details, inner)
        { }

        public BankIdApiException(ErrorCode errorCode, string details)
            : this(errorCode, details, null)
        { }

        public BankIdApiException(ErrorCode errorCode, string details, Exception inner)
            : base($"{errorCode}: {details}", inner)
        {
            ErrorCode = errorCode;
            Details = details;
        }

        public ErrorCode ErrorCode { get; }
        public string Details { get; }
    }
}
