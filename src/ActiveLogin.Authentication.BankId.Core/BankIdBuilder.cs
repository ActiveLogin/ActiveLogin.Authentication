using System.Security.Authentication;

using ActiveLogin.Authentication.BankId.Api;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

internal class BankIdBuilder : IBankIdBuilder
{
    public IServiceCollection Services { get; }

    private readonly List<Action<HttpClient>> _httpClientConfigurators = new();
    private readonly List<Action<SocketsHttpHandler>> _httpClientHandlerConfigurators = new();

    public BankIdBuilder(IServiceCollection services)
    {
        Services = services;

        ConfigureHttpClient(httpClient => httpClient.BaseAddress = BankIdUrls.ProductionApiBaseUrl);
        ConfigureHttpClientHandler(httpClientHandler => httpClientHandler.SslOptions.EnabledSslProtocols = SslProtocols.Tls12);
    }

    public void ConfigureHttpClient(Action<HttpClient> configureHttpClient)
    {
        _httpClientConfigurators.Add(configureHttpClient);
    }

    public void ConfigureHttpClientHandler(Action<SocketsHttpHandler> configureHttpClientHandler)
    {
        _httpClientHandlerConfigurators.Add(configureHttpClientHandler);
    }

    public void EnableHttpBankIdApiClient()
    {
        Services.AddHttpClient<IBankIdApiClient, BankIdApiClient>(httpClient =>
            {
                _httpClientConfigurators.ForEach(configurator => configurator(httpClient));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new SocketsHttpHandler();
                _httpClientHandlerConfigurators.ForEach(configurator => configurator(httpClientHandler));
                return httpClientHandler;
            });
    }
}
