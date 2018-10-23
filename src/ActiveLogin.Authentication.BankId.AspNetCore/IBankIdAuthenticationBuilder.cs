using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public interface IBankIdAuthenticationBuilder
    {
        AuthenticationBuilder AuthenticationBuilder { get; }

        void ConfigureBankIdHttpClient(Action<HttpClient> configureHttpClient);
        void ConfigureBankIdHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler);
        void EnableBankIdHttpClient();
    }
}