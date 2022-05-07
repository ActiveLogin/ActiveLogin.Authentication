using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiScriptConfiguration
{
    internal BankIdUiScriptConfiguration()
    {
    }

    [JsonPropertyName("statusRefreshIntervalMs")]
    public int StatusRefreshIntervalMs { get; init; }

    [JsonPropertyName("qrCodeRefreshIntervalMs")]
    public int QrCodeRefreshIntervalMs { get; init; }


    [JsonPropertyName("bankIdInitializeApiUrl")]
    public string BankIdInitializeApiUrl { get; init; } = string.Empty;

    [JsonPropertyName("bankIdStatusApiUrl")]
    public string BankIdStatusApiUrl { get; init; } = string.Empty;

    [JsonPropertyName("bankIdQrCodeApiUrl")]
    public string BankIdQrCodeApiUrl { get; init; } = string.Empty;

    [JsonPropertyName("bankIdCancelApiUrl")]
    public string BankIdCancelApiUrl { get; init; } = string.Empty;


    [JsonPropertyName("initialStatusMessage")]
    public string InitialStatusMessage { get; init; } = string.Empty;

    [JsonPropertyName("unknownErrorMessage")]
    public string UnknownErrorMessage { get; init; } = string.Empty;

    [JsonPropertyName("unsupportedBrowserErrorMessage")]
    public string UnsupportedBrowserErrorMessage { get; init; } = string.Empty;
}
