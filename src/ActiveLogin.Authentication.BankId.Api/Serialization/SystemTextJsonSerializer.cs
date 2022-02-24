using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.Api.Serialization
{
    internal static class SystemTextJsonSerializer
    {
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

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Serialize(value, value.GetType(), options);
        }
    }
}
