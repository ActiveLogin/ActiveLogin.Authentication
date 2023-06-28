using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

public interface IBankIdBuilder
{
    IServiceCollection Services { get; }

    /// <summary>
    /// Configure the <see cref="HttpClient"/> used for making calls to the BankID App API.
    /// </summary>
    /// <param name="configureHttpClient"></param>
    void ConfigureAppApiHttpClient(Action<IServiceProvider, HttpClient> configureHttpClient);

    /// <summary>
    /// Configure the <see cref="SocketsHttpHandler"/> used by <see cref="HttpClient"/> for making calls to the BankID App API.
    /// </summary>
    /// <param name="configureHttpClientHandler"></param>
    void ConfigureAppApiHttpClientHandler(Action<IServiceProvider, SocketsHttpHandler> configureHttpClientHandler);

    /// <summary>
    /// Configure the <see cref="HttpClient"/> used for making calls to the BankID Verify API.
    /// </summary>
    /// <param name="configureHttpClient"></param>
    void ConfigureVerifyApiHttpClient(Action<IServiceProvider, HttpClient> configureHttpClient);

    /// <summary>
    /// Configure the <see cref="SocketsHttpHandler"/> used by <see cref="HttpClient"/> for making calls to the BankID Verify API.
    /// </summary>
    /// <param name="configureHttpClientHandler"></param>
    void ConfigureVerifyApiHttpClientHandler(Action<IServiceProvider, SocketsHttpHandler> configureHttpClientHandler);
}
