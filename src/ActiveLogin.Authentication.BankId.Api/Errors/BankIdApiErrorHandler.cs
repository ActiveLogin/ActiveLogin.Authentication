using System;
using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Errors
{
    internal class BankIdApiErrorHandler
    {
        public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponseMessage)
        {
            var error = await TryGetError(httpResponseMessage);

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

        private static async Task<Error> TryGetError(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerialization.Deserialize<Error>(await httpResponseMessage.Content.ReadAsStringAsync());
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