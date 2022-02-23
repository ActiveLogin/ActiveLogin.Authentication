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
                    var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var deserialized = await SystemRuntimeJsonSerializer.DeserializeAsync<Error>(content);

                    if (deserialized == null)
                    {
                        throw new Exception("Could not deserialize JSON response");
                    }

                    return deserialized;
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
