using System;

namespace ActiveLogin.Authentication.GrandId.Api.Models;

internal static class ErrorExtensions
{
    public static ErrorCode GetErrorCode(this Error error)
    {
        return Enum.TryParse<ErrorCode>(error.Code, true, out var errorCode) ? errorCode : ErrorCode.Unknown;
    }
}