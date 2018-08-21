using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    internal static class GrandIdHttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient httpClient, string action, IJsonSerializer jsonSerializer)
        {
            var callUrl = httpClient.BaseAddress + action;
            var httpResponseMessage = await httpClient.GetAsync(callUrl);
            return jsonSerializer.Deserialize<TResult>(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }
    }
}
