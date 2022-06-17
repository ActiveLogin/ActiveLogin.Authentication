using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

internal class Error
{
    public static Error Empty = new(string.Empty, string.Empty);

    public Error(string errorCode, string details)
    {
        ErrorCode = errorCode;
        Details = details;
    }

    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; }

    [JsonPropertyName("details")]
    public string Details { get; }
}
