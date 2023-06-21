using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

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

        ConfigureHttpClient((sp, httpClient) => httpClient.BaseAddress = BankIdUrls.AppApiProductionBaseUrl);
        ConfigureHttpClientHandler((sp, httpClientHandler) =>
        {
            httpClientHandler.SslOptions.EnabledSslProtocols = SslProtocols.Tls12;

            // From .NET 7 it seems as we have to implement this to get
            httpClientHandler.SslOptions.LocalCertificateSelectionCallback = (sender, host, certificates, certificate, issuers) => GetFirstCertificate(certificates)!;
        });
    }

    private static X509Certificate? GetFirstCertificate(X509CertificateCollection localcertificates)
    {
        return localcertificates.Count == 0 ? null : localcertificates[0];
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
        Services.AddHttpClient<IBankIdAppApiClient, BankIdAppApiClient>((sp, httpClient) =>
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
