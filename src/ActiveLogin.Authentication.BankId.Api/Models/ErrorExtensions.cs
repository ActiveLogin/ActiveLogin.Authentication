using System;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    internal static class ErrorExtensions
    {
        public static ErrorCode GetErrorCode(this Error error)
        {
            return Enum.TryParse(error.ErrorCode, true, out ErrorCode errorCode) ? errorCode : ErrorCode.Unknown;
        }
    }
}
