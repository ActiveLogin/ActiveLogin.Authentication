namespace ActiveLogin.Authentication.BankId.Api.Models;

internal static class ErrorExtensions
{
    public static ErrorCode GetErrorCode(this Error error)
    {
        return BankIdApiConverters.ParseErrorCode(error.ErrorCode);
    }
}
