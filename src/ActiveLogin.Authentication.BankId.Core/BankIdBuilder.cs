using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using ActiveLogin.Authentication.BankId.Api;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

internal class BankIdBuilder : IBankIdBuilder
{
    public IServiceCollection Services { get; }

    private readonly List<Action<IServiceProvider, HttpClient>> _httpClientAppApiConfigurators = new();
    private readonly List<Action<IServiceProvider, SocketsHttpHandler>> _httpClientHandlerAppApiConfigurators = new();

    private readonly List<Action<IServiceProvider, HttpClient>> _httpClientVerifyApiConfigurators = new();
    private readonly List<Action<IServiceProvider, SocketsHttpHandler>> _httpClientHandlerVerifyApiConfigurators = new();


    public BankIdBuilder(IServiceCollection services)
    {
        Services = services;

        ConfigureAppApiHttpClient((sp, httpClient) => httpClient.BaseAddress = BankIdUrls.AppApiProductionBaseUrl);
        ConfigureAppApiHttpClientHandler(ConfigureHttpClientHandlerForSsl);

        ConfigureVerifyApiHttpClient((sp, httpClient) => httpClient.BaseAddress = BankIdUrls.VerifyApiProductionBaseUrl);
        ConfigureVerifyApiHttpClientHandler(ConfigureHttpClientHandlerForSsl);
    }

    private void ConfigureHttpClientHandlerForSsl(IServiceProvider sp, SocketsHttpHandler httpClientHandler)
    {
        httpClientHandler.SslOptions.EnabledSslProtocols = SslProtocols.Tls12;

        // From .NET 7 it seems as we have to implement this to get
        httpClientHandler.SslOptions.LocalCertificateSelectionCallback = (sender, host, certificates, certificate, issuers) => GetFirstCertificate(certificates)!;
    }

    private static X509Certificate? GetFirstCertificate(X509CertificateCollection localcertificates)
    {
        return localcertificates.Count == 0 ? null : localcertificates[0];
    }

    public void ConfigureAppApiHttpClient(Action<IServiceProvider, HttpClient> configureHttpClient)
    {
        _httpClientAppApiConfigurators.Add(configureHttpClient);
    }

    public void ConfigureAppApiHttpClientHandler(Action<IServiceProvider, SocketsHttpHandler> configureHttpClientHandler)
    {
        _httpClientHandlerAppApiConfigurators.Add(configureHttpClientHandler);
    }

    public void ConfigureVerifyApiHttpClient(Action<IServiceProvider, HttpClient> configureHttpClient)
    {
        _httpClientVerifyApiConfigurators.Add(configureHttpClient);
    }

    public void ConfigureVerifyApiHttpClientHandler(Action<IServiceProvider, SocketsHttpHandler> configureHttpClientHandler)
    {
        _httpClientHandlerVerifyApiConfigurators.Add(configureHttpClientHandler);
    }

    public void AfterConfiguration()
    {
        Services.AddHttpClient<IBankIdAppApiClient, BankIdAppApiClient>((sp, httpClient) =>
            {
                _httpClientAppApiConfigurators.ForEach(configurator => configurator(sp, httpClient));
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var httpClientHandler = new SocketsHttpHandler();
                _httpClientHandlerAppApiConfigurators.ForEach(configurator => configurator(sp, httpClientHandler));
                return httpClientHandler;
            });

        Services.AddHttpClient<IBankIdVerifyApiClient, BankIdVerifyApiClient>((sp, httpClient) =>
            {
                _httpClientVerifyApiConfigurators.ForEach(configurator => configurator(sp, httpClient));
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var httpClientHandler = new SocketsHttpHandler();
                _httpClientHandlerVerifyApiConfigurators.ForEach(configurator => configurator(sp, httpClientHandler));
                return httpClientHandler;
            });
    }
}
