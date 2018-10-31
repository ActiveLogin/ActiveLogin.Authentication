using System;
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
            var httpResponseMessage = await GetHttpResponseAsync(url, request, httpClient.PostAsync).ConfigureAwait(false);
            return SystemRuntimeJsonSerializer.Deserialize<TResult>(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }

        private static async Task<HttpResponseMessage> GetHttpResponseAsync<TRequest>(string url, TRequest request, Func<string, HttpContent, Task<HttpResponseMessage>> httpRequest)
        {
            var requestJson = SystemRuntimeJsonSerializer.Serialize(request);
            var requestContent = GetJsonStringContent(requestJson);

            var httpResponseMessage = await httpRequest(CleanUrl(url), requestContent).ConfigureAwait(false);
            await BankIdApiErrorHandler.EnsureSuccessAsync(httpResponseMessage).ConfigureAwait(false);
            return httpResponseMessage;
        }

        private static StringContent GetJsonStringContent(string requestJson)
        {
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");
            requestContent.Headers.ContentType.CharSet = string.Empty;
            return requestContent;
        }

        private static string CleanUrl(string url)
        {
            return url.TrimStart('/');
        }
    }
}
