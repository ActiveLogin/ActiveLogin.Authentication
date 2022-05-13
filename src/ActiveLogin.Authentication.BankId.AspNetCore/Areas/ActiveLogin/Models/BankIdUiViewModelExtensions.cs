using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public static class BankIdUiViewModelExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static string BankUiScriptConfigurationJson(this BankIdUiViewModel model) => SerializeToJson(model.BankUiScriptConfiguration);

    public static string BankIdUiScriptInitStateJson(this BankIdUiViewModel model) => SerializeToJson(model.BankIdUiScriptInitState);

    public static string BankIdUiSignDataAsJson(this BankIdUiViewModel model) => SerializeToJson(model.BankIdUiSignData);

    private static string SerializeToJson<T>(T value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return JsonSerializer.Serialize(value, value.GetType(), JsonSerializerOptions);
    }
}
