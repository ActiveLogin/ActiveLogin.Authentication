using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    internal class Error
    {
        public static Error Empty = new Error(string.Empty, string.Empty);

        public Error(string errorCode, string details)
        {
            ErrorCode = errorCode;
            Details = details;
        }

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; private set; }

        [JsonPropertyName("details")]
        public string Details { get; private set; }
    }
}
