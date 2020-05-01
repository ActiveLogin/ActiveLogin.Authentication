using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public interface IBankIdBuilder
    {
        AuthenticationBuilder AuthenticationBuilder { get; }

        void ConfigureHttpClient(Action<HttpClient> configureHttpClient);
        void ConfigureHttpClientHandler(Action<SocketsHttpHandler> configureHttpClientHandler);
    }
}
