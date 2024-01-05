using System.Security.Cryptography.X509Certificates;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.Cryptography;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.ResultStore;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;
public static class IBankIdBuilderExtensions
{
    /// <summary>
    /// Use client certificate for authenticating against the BankID API.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureClientCertificate">The certificate to use.</param>
    /// <returns></returns>
    public static IBankIdBuilder AddClientCertificate(this IBankIdBuilder builder, Func<X509Certificate2> configureClientCertificate)
    {
        builder.ConfigureAppApiHttpClientHandler((sp, httpClientHandler) =>
        {
            var clientCertificate = configureClientCertificate();
            httpClientHandler.SslOptions.ClientCertificates ??= new X509Certificate2Collection();
            httpClientHandler.SslOptions.ClientCertificates.Add(clientCertificate);
        });

        return builder;
    }

    /// <summary>
    /// Add client certificate for authenticating against the BankID API to the list of available certificates for the http client handler to choose from.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureClientCertificate">The certificate to add.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseClientCertificate(this IBankIdBuilder builder, Func<X509Certificate2> configureClientCertificate)
    {
        builder.ConfigureAppApiHttpClientHandler(ConfigureHttpClientHandler);
        builder.ConfigureVerifyApiHttpClientHandler(ConfigureHttpClientHandler);

        return builder;

        void ConfigureHttpClientHandler(IServiceProvider sp, SocketsHttpHandler httpClientHandler)
        {
            var clientCertificate = configureClientCertificate();
            httpClientHandler.SslOptions.ClientCertificates = new X509Certificate2Collection { clientCertificate };
        }
    }

    /// <summary>
    /// Use this root certificate for verifying the certificate of BankID API.
    /// Use only if the root certificate can't be installed on the machine.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureRootCaCertificate">The root certificate provided by BankID (*.crt)</param>
    /// <returns></returns>
    public static IBankIdBuilder UseRootCaCertificate(this IBankIdBuilder builder, Func<X509Certificate2> configureRootCaCertificate)
    {
        builder.ConfigureAppApiHttpClientHandler(ConfigureHttpClientHandler);
        builder.ConfigureVerifyApiHttpClientHandler(ConfigureHttpClientHandler);

        return builder;

        void ConfigureHttpClientHandler(IServiceProvider sp, SocketsHttpHandler httpClientHandler)
        {
            var rootCaCertificate = configureRootCaCertificate();
            var validator = new X509CertificateChainValidator(rootCaCertificate);
            httpClientHandler.SslOptions.RemoteCertificateValidationCallback = validator.Validate;
        }
    }

    /// <summary>
    /// Use this root certificate for verifying the certificate of BankID API.
    /// Use only if the root certificate can't be installed on the machine.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="certificateFilePath">The file path to the root certificate provided by BankID (*.crt)</param>
    /// <returns></returns>
    public static IBankIdBuilder UseRootCaCertificate(this IBankIdBuilder builder, string certificateFilePath)
    {
        builder.UseRootCaCertificate(() => new X509Certificate2(certificateFilePath));

        return builder;
    }

    /// <summary>
    /// Add a custom event listener.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddEventListener<TImplementation>(this IBankIdBuilder builder) where TImplementation : class, IBankIdEventListener
    {
        builder.Services.AddTransient<IBankIdEventListener, TImplementation>();

        return builder;
    }

    /// <summary>
    /// Add an event listener that will serialize and write all events to debug.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddDebugEventListener(this IBankIdBuilder builder)
    {
        builder.AddEventListener<BankIdDebugEventListener>();

        return builder;
    }

    /// <summary>
    /// Adds a class that will be called when BankID returns a valid signed in user.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddResultStore<TImplementation>(this IBankIdBuilder builder) where TImplementation : class, IBankIdResultStore
    {
        builder.Services.AddTransient<IBankIdResultStore, TImplementation>();

        return builder;
    }

    /// <summary>
    /// Adds a class to resolve custom browser config.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomBrowser<TImplementation>(this IBankIdBuilder builder) where TImplementation : class, IBankIdLauncherCustomBrowser
    {
        builder.Services.AddTransient<IBankIdLauncherCustomBrowser, TImplementation>();

        return builder;
    }

    /// <summary>
    /// Adds support for a custom browser (like a third party app).
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="getResult"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomBrowserByContext(this IBankIdBuilder builder, Func<BankIdLauncherCustomBrowserContext, bool> isApplicable, Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> getResult)
    {
        builder.Services.AddTransient<IBankIdLauncherCustomBrowser>(x => new BankIdLauncherCustomBrowserByContext(isApplicable, getResult));

        return builder;
    }

    internal static IBankIdBuilder UseEnvironment(this IBankIdBuilder builder, Uri appApiBaseUrl, Uri verifyApiBaseUrl, string environment)
    {
        SetActiveLoginContext(builder.Services, environment, BankIdUrls.AppApiVersion, BankIdUrls.VerifyApiVersion);

        builder.ConfigureAppApiHttpClient((sp, httpClient) =>
        {
            httpClient.BaseAddress = appApiBaseUrl;
        });

        builder.ConfigureVerifyApiHttpClient((sp, httpClient) =>
        {
            httpClient.BaseAddress = verifyApiBaseUrl;
        });

        builder.Services.AddTransient<IBankIdLauncher, BankIdLauncher>();

        if (builder is BankIdBuilder bankIdBuilder)
        {
            bankIdBuilder.AfterConfiguration();
        }

        return builder;
    }

    /// <summary>
    /// Use the BankID test environment (https://appapi2.test.bankid.com/rp/vX/)
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="useBankIdRootCertificate">Use the BankID root certificate (for test) from the BankID documentation.</param>
    /// <param name="useBankIdClientCertificate">Use the BankID client certificate (for test) from the BankID documentation.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseTestEnvironment(this IBankIdBuilder builder, bool useBankIdRootCertificate = true, bool useBankIdClientCertificate = true)
    {
        builder.UseEnvironment(BankIdUrls.AppApiTestBaseUrl, BankIdUrls.VerifyApiTestBaseUrl, BankIdEnvironments.Test);

        if (useBankIdRootCertificate)
        {
            var cert = BankIdCertificates.BankIdApiRootCertificateTest;
            builder.UseRootCaCertificate(() => cert);
        }

        if (useBankIdClientCertificate)
        {
            var cert = BankIdCertificates.BankIdApiClientCertificateTest;
            builder.UseClientCertificate(() => cert);
        }

        return builder;
    }

    /// <summary>
    /// Use the BankID production environment (https://appapi2.bankid.com/rp/vX/)
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="useBankIdRootCertificate">Use the BankID root certificate (for production) from the BankID documentation.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseProductionEnvironment(this IBankIdBuilder builder, bool useBankIdRootCertificate = true)
    {
        builder.UseEnvironment(BankIdUrls.AppApiProductionBaseUrl, BankIdUrls.VerifyApiProductionBaseUrl, BankIdEnvironments.Production);

        if (useBankIdRootCertificate)
        {
            var cert = BankIdCertificates.BankIdApiRootCertificateProd;
            builder.UseRootCaCertificate(() => cert);
        }

        return builder;
    }

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder)
    {
        return UseSimulatedEnvironment(builder,
            x => new BankIdSimulatedAppApiClient(),
            x => new BankIdSimulatedVerifyApiClient()
        );
    }

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="givenName">Fake given name</param>
    /// <param name="surname">Fake surname</param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname)
    {
        return UseSimulatedEnvironment(builder,
            x => new BankIdSimulatedAppApiClient(givenName, surname),
            x => new BankIdSimulatedVerifyApiClient(givenName, surname)
        );
    }

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="givenName">Fake given name</param>
    /// <param name="surname">Fake surname</param>
    /// <param name="personalIdentityNumber">Fake personal identity number</param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname, string personalIdentityNumber)
    {
        return UseSimulatedEnvironment(builder,
            x => new BankIdSimulatedAppApiClient(givenName, surname, personalIdentityNumber),
            x => new BankIdSimulatedVerifyApiClient(givenName, surname, personalIdentityNumber)
        );
    }

    private static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, Func<IServiceProvider, IBankIdAppApiClient> bankIdSimulatedAppApiClient, Func<IServiceProvider, IBankIdVerifyApiClient> bankIdSimulatedVerifyApiClient)
    {
        SetActiveLoginContext(builder.Services, BankIdEnvironments.Simulated, BankIdSimulatedAppApiClient.Version, BankIdSimulatedVerifyApiClient.Version);

        builder.Services.AddSingleton(bankIdSimulatedAppApiClient);
        builder.Services.AddSingleton(bankIdSimulatedVerifyApiClient);
        builder.Services.AddSingleton<IBankIdLauncher, BankIdDevelopmentLauncher>();

        return builder;
    }

    private static void SetActiveLoginContext(IServiceCollection services, string environment, string appApiVersion, string verifyApiVersion)
    {
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.BankIdApiEnvironment = environment;
            context.BankIdAppApiVersion = appApiVersion;
            context.BankIdVerifyApiVersion = verifyApiVersion;
        });
    }
}
