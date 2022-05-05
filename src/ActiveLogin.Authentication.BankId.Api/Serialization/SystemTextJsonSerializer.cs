using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.Api.Serialization;

internal static class SystemTextJsonSerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static ValueTask<T?> DeserializeAsync<T>(Stream json)
    {
        return JsonSerializer.DeserializeAsync<T>(json);
    }

    public static string Serialize<T>(T value)
    {
        if (value == null)
        {
            return string.Empty;
        }
        
        return JsonSerializer.Serialize(value, value.GetType(), JsonSerializerOptions);
    }
}
