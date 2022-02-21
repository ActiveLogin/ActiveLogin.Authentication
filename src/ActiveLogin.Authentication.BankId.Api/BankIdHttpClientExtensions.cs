using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api.Errors;
using ActiveLogin.Authentication.BankId.Api.Serialization;

namespace ActiveLogin.Authentication.BankId.Api
{
    internal static class BankIdHttpClientExtensions
    {
        private const string JsonMediaType = "application/json";

        public static async Task<TResult> PostAsync<TRequest, TResult>(this HttpClient httpClient, string url, TRequest request)
        {
            var requestJson = SystemRuntimeJsonSerializer.Serialize(request);
            var requestContent = GetJsonStringContent(requestJson);

            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await httpClient.PostAsync(url, requestContent).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw BankIdApiException.Unknown(e);
            }

            await BankIdApiErrorHandler.EnsureSuccessAsync(httpResponseMessage).ConfigureAwait(false);
            var content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var deserialized = SystemRuntimeJsonSerializer.Deserialize<TResult>(content);

            if (deserialized == null)
            {
                throw new Exception("Could not deserialize JSON response");
            }

            return deserialized;
        }

        private static StringContent GetJsonStringContent(string requestJson)
        {
            var requestContent = new StringContent(requestJson, Encoding.Default, JsonMediaType);
            requestContent.Headers.ContentType.CharSet = string.Empty;
            return requestContent;
        }
    }
}
