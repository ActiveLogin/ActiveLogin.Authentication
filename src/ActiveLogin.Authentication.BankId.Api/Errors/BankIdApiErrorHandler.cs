using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.Common.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.Api.Errors
{
    internal class BankIdApiErrorHandler
    {
        public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponseMessage)
        {
            Error error = await TryGetErrorAsync(httpResponseMessage).ConfigureAwait(false);

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                if (error != null)
                    throw new BankIdApiException(error, e);

                throw new BankIdApiException(ErrorCode.Unknown, "Unknown error", e);
            }
        }

        private static async Task<Error> TryGetErrorAsync(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    string content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return SystemRuntimeJsonSerializer.Deserialize<Error>(content);
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
