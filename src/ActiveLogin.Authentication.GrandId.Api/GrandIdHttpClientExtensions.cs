using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    internal static class GrandIdHttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient httpClient, string url)
        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
            Stream content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return SystemRuntimeJsonSerializer.Deserialize<TResult>(content);
        }

        public static async Task<TResult> PostAsync<TResult>(this HttpClient httpClient, string url,
            Dictionary<string, string> postData)
        {
            FormUrlEncodedContent requestContent = GetFormUrlEncodedContent<TResult>(postData);

            HttpResponseMessage httpResponseMessage =
                await httpClient.PostAsync(url, requestContent).ConfigureAwait(false);
            Stream content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return SystemRuntimeJsonSerializer.Deserialize<TResult>(content);
        }

        private static FormUrlEncodedContent GetFormUrlEncodedContent<TResult>(Dictionary<string, string> postData)
        {
            var requestContent = new FormUrlEncodedContent(postData);

            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return requestContent;
        }
    }
}
