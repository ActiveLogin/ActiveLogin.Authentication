using System.Security.Authentication;

using ActiveLogin.Authentication.BankId.Api;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

internal class BankIdBuilder : IBankIdBuilder
{
    public IServiceCollection Services { get; }

    private readonly List<Action<IServiceProvider, HttpClient>> _httpClientConfigurators = new();
    private readonly List<Action<IServiceProvider, SocketsHttpHandler>> _httpClientHandlerConfigurators = new();

    public BankIdBuilder(IServiceCollection services)
    {
        Services = services;

        ConfigureHttpClient((sp, httpClient) => httpClient.BaseAddress = BankIdUrls.ProductionApiBaseUrl);
        ConfigureHttpClientHandler((sp, httpClientHandler) => httpClientHandler.SslOptions.EnabledSslProtocols = SslProtocols.Tls12);
    }

    public void ConfigureHttpClient(Action<IServiceProvider, HttpClient> configureHttpClient)
    {
        _httpClientConfigurators.Add(configureHttpClient);
    }

    public void ConfigureHttpClientHandler(Action<IServiceProvider, SocketsHttpHandler> configureHttpClientHandler)
    {
        _httpClientHandlerConfigurators.Add(configureHttpClientHandler);
    }

    public void AfterConfiguration()
    {
        Services.AddHttpClient<IBankIdApiClient, BankIdApiClient>((sp, httpClient) =>
            {
                _httpClientConfigurators.ForEach(configurator => configurator(sp, httpClient));
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var httpClientHandler = new SocketsHttpHandler();
                _httpClientHandlerConfigurators.ForEach(configurator => configurator(sp, httpClientHandler));
                return httpClientHandler;
            });
    }
}
