using System;
using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Errors
{
    internal class BankIdApiErrorHandler
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
                    throw new BankIdApiException(error, e);
                }

                throw new BankIdApiException("Unknown error", e);
            }
        }

        private static async Task<Error> TryGetErrorAsync(HttpResponseMessage httpResponseMessage, IJsonSerializer jsonSerializer)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return jsonSerializer.Deserialize<Error>(content);
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