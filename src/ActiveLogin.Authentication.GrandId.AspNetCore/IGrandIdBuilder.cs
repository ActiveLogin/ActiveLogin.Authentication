using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore;

public interface IGrandIdBuilder
{
    AuthenticationBuilder AuthenticationBuilder { get; }

    void ConfigureHttpClient(Action<HttpClient> configureHttpClient);
    void ConfigureHttpClientHandler(Action<SocketsHttpHandler> configureHttpClientHandler);
    void EnableHttpClient();
}