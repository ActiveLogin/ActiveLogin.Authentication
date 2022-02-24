using System.IO;
using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal static class SystemTextJsonSerializer
    {
        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Serialize(value, value.GetType(), options);
        }
    }
}
