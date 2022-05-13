using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiSignData
{
    internal BankIdUiSignData()
    {
    }

    [JsonPropertyName("userVisibleData")]
    public string UserVisibleData { get; init; } = string.Empty;
}
