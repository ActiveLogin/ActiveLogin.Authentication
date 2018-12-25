using System;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    internal static class ErrorExtensions
    {
        public static ErrorCode GetErrorCode(this Error error)
        {
            return Enum.TryParse<ErrorCode>(error.ErrorCode, true, out var errorCode) ? errorCode : ErrorCode.Unknown;
        }
    }
}