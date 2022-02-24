using System;
using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Errors
{
    internal static class BankIdApiErrorHandler
    {
        public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponseMessage)
        {
            var error = await TryGetErrorAsync(httpResponseMessage).ConfigureAwait(false);

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                if (error != Error.Empty)
                {
                    throw new BankIdApiException(error, e);
                }

                throw BankIdApiException.Unknown(e);
            }
        }

        private static async Task<Error> TryGetErrorAsync(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    var deserializedContent = await SystemTextJsonSerializer.DeserializeAsync<Error>(contentStream).ConfigureAwait(false);

                    if (deserializedContent == null)
                    {
                        throw new Exception("Could not deserialize JSON response");
                    }

                    return deserializedContent;
                }
                catch (Exception)
                {
                    // Intentionally left empty
                }
            }

            return Error.Empty;
        }
    }
}
