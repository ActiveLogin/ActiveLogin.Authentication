using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.Common.Serialization
{
    public static class SystemRuntimeJsonSerializer
    {
        public static async Task<T> DeserializeAsync<T>(string json)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
        }

        public static async Task<string> SerializeAsync<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, value).ConfigureAwait(false);
            var json = stream.ToArray();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}
