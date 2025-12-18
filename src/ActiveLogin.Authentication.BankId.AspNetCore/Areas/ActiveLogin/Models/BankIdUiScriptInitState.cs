using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiScriptInitState
{
    internal BankIdUiScriptInitState()
    {
    }

    [JsonPropertyName("antiXsrfRequestToken")]
    public string AntiXsrfRequestToken { get; init; } = string.Empty;

    [JsonPropertyName("returnUrl")]
    public string ReturnUrl { get; init; } = string.Empty;

    [JsonPropertyName("cancelReturnUrl")]
    public string CancelReturnUrl { get; init; } = string.Empty;
}
