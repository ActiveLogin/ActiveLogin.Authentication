using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

public interface IBankIdBuilder
{
    IServiceCollection Services { get; }

    /// <summary>
    /// Configure the <see cref="HttpClient"/> used for making calls to the BankID API.
    /// </summary>
    /// <param name="configureHttpClient"></param>
    void ConfigureHttpClient(Action<IServiceProvider, HttpClient> configureHttpClient);

    /// <summary>
    /// Configure the <see cref="SocketsHttpHandler"/> used by <see cref="HttpClient"/> for making calls to the BankID API.
    /// </summary>
    /// <param name="configureHttpClientHandler"></param>
    void ConfigureHttpClientHandler(Action<IServiceProvider, SocketsHttpHandler> configureHttpClientHandler);
}
