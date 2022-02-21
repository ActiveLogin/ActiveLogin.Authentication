using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using ActiveLogin.Authentication.GrandId.Api.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    internal static class GrandIdHttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient httpClient, string url)
        {
            var httpResponseMessage = await httpClient.GetAsync(url);
            var content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var deserialized = SystemRuntimeJsonSerializer.Deserialize<TResult>(content);

            if (deserialized == null)
            {
                throw new Exception("Could not deserialize JSON response");
            }

            return deserialized;
        }

        public static async Task<TResult> PostAsync<TResult>(this HttpClient httpClient, string url, Dictionary<string, string?> postData)
        {
            var requestContent = GetFormUrlEncodedContent(postData);

            var httpResponseMessage = await httpClient.PostAsync(url, requestContent).ConfigureAwait(false);
            var content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var deserialized = SystemRuntimeJsonSerializer.Deserialize<TResult>(content);

            if (deserialized == null)
            {
                throw new Exception("Could not deserialize JSON response");
            }

            return deserialized;
        }

        private static FormUrlEncodedContent GetFormUrlEncodedContent(Dictionary<string, string?> postData)
        {
            var requestContent = new FormUrlEncodedContent(postData);

            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return requestContent;
        }
    }
}
