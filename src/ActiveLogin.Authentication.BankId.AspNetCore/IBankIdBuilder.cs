using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public interface IBankIdBuilder
{
    AuthenticationBuilder AuthenticationBuilder { get; }

    /// <summary>
    /// Configure the <see cref="HttpClient"/> used for making calls to the BankID API.
    /// </summary>
    /// <param name="configureHttpClient"></param>
    void ConfigureHttpClient(Action<HttpClient> configureHttpClient);

    /// <summary>
    /// Configure the <see cref="SocketsHttpHandler"/> used by <see cref="HttpClient"/> for making calls to the BankID API.
    /// </summary>
    /// <param name="configureHttpClientHandler"></param>
    void ConfigureHttpClientHandler(Action<SocketsHttpHandler> configureHttpClientHandler);
}
