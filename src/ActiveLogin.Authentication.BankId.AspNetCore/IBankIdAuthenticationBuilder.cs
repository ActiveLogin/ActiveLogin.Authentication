using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public interface IBankIdAuthenticationBuilder
    {
        AuthenticationBuilder AuthenticationBuilder { get; }

        void ConfigureHttpClient(Action<HttpClient> configureHttpClient);
        void ConfigureHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler);
        void EnableHttpClient();
    }
}
