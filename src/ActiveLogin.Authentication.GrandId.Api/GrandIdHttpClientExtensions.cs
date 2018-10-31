using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    internal static class GrandIdHttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient httpClient, string url)
        {
            var httpResponseMessage = await httpClient.GetAsync(url);
            var content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return SystemRuntimeJsonSerializer.Deserialize<TResult>(content);
        }
    }
}
