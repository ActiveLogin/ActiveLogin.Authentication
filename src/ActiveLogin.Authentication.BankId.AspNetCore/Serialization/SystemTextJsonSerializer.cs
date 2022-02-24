using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal static class SystemTextJsonSerializer
    {
        public static async Task<string> SerializeAsync<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = string.Empty;
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, value, value.GetType(), options);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
            }

            return json;
        }
    }
}
