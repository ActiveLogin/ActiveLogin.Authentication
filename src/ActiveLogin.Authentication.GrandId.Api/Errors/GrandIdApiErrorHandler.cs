using System;
using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Errors
{
    internal class GrandIdApiErrorHandler
    {
        public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponseMessage, IJsonSerializer jsonSerializer)
        {
            var error = await TryGetErrorAsync(httpResponseMessage, jsonSerializer).ConfigureAwait(false);

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                if (error != null)
                {
                    throw new GrandIdApiException(error, e);
                }

                throw new GrandIdApiException("Unknown error", e);
            }
        }

        private static async Task<Error> TryGetErrorAsync(HttpResponseMessage httpResponseMessage, IJsonSerializer jsonSerializer)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    return jsonSerializer.Deserialize<Error>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
                catch (Exception)
                {
                    // Intentionally left empty
                }
            }

            return null;
        }
    }
}