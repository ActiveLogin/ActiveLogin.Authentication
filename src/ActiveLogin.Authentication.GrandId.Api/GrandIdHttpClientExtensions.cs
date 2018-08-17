using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Errors;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    internal static class GrandIdHttpClientExtensions
    {
        public static async Task<TResult> PostAsync<TRequest, TResult>(this HttpClient httpClient, string url, TRequest request, IJsonSerializer jsonSerializer)
        {
            var httpResponseMessage = await GetHttpResponseAsync(url, request, httpClient.PostAsync, jsonSerializer).ConfigureAwait(false);
            return jsonSerializer.Deserialize<TResult>(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }

        public static async Task<TResult> getAsync<TRequest, TResult>(this HttpClient httpClient, string url, TRequest request, IJsonSerializer jsonSerializer)
        {
            var callUrl = httpClient.BaseAddress + url;
            var httpResponseMessage = await httpClient.GetAsync(callUrl);
            return jsonSerializer.Deserialize<TResult>(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }

        private static async Task<HttpResponseMessage> GetHttpResponseAsync<TRequest>(string url, TRequest request, Func<string, HttpContent, Task<HttpResponseMessage>> httpRequest, IJsonSerializer jsonSerializer)
        {
            var requestJson = jsonSerializer.Serialize(request);
            var requestContent = GetJsonStringContent(requestJson);

            var httpResponseMessage = await httpRequest(CleanUrl(url), requestContent).ConfigureAwait(false);
            await GrandIdApiErrorHandler.EnsureSuccessAsync(httpResponseMessage, jsonSerializer).ConfigureAwait(false);
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
