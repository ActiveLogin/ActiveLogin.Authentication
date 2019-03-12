using System;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    internal static class ErrorExtensions
    {
        public static ErrorCode GetErrorCode(this Error error)
        {
            return Enum.TryParse(error.Code, true, out ErrorCode errorCode)
                ? errorCode
                : ErrorCode.Unknown;
        }
    }
}
