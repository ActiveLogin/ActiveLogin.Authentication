using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiSignData
{
    internal BankIdUiSignData()
    {
    }

    [JsonPropertyName("userVisibleData")]
    public string UserVisibleData { get; init; } = string.Empty;

    [JsonPropertyName("userVisibleDataFormat")]
    public string? UserVisibleDataFormat { get; init; } = string.Empty;
}
