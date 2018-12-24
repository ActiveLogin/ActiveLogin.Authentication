﻿using System;
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
            var error = await TryGetErrorAsync(httpResponseMessage).ConfigureAwait(false);

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

                throw new BankIdApiException(new Error("Unknown", "Unknown error"), e);
            }
        }

        private static async Task<Error> TryGetErrorAsync(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
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