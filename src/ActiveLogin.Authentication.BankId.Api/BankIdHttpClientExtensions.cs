using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Errors;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.BankId.Api
{
    internal static class BankIdHttpClientExtensions
    {
        public static async Task<TResult> PostAsync<TRequest, TResult>(this HttpClient httpClient, string url, TRequest request)
        {
            var requestJson = await SystemRuntimeJsonSerializer.SerializeAsync(request);
            var requestContent = GetJsonStringContent(requestJson);

            var httpResponseMessage = await httpClient.PostAsync(url, requestContent).ConfigureAwait(false);
            await BankIdApiErrorHandler.EnsureSuccessAsync(httpResponseMessage).ConfigureAwait(false);
            var content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return await SystemRuntimeJsonSerializer.DeserializeAsync<TResult>(content);
        }

        private static StringContent GetJsonStringContent(string requestJson)
        {
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");
            requestContent.Headers.ContentType.CharSet = string.Empty;
            return requestContent;
        }
    }
}
