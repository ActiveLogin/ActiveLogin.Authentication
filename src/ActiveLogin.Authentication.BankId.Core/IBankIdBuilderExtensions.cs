using System.Security.Cryptography.X509Certificates;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Cryptography;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.ResultSTore;
using ActiveLogin.Authentication.BankId.Core.UserData;

namespace Microsoft.Extensions.DependencyInjection;
public static class IBankIdBuilderExtensions
{
    /// <summary>
    /// Use client certificate for authenticating against the BankID API.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureClientCertificate">The certificate to use.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseClientCertificate(this IBankIdBuilder builder, Func<X509Certificate2> configureClientCertificate)
    {
        builder.ConfigureHttpClientHandler(httpClientHandler =>
        {
            var clientCertificate = configureClientCertificate();
            httpClientHandler.SslOptions.ClientCertificates ??= new X509Certificate2Collection();
            httpClientHandler.SslOptions.ClientCertificates.Add(clientCertificate);
        });

        return builder;
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
        builder.ConfigureHttpClientHandler(httpClientHandler =>
        {
            var rootCaCertificate = configureRootCaCertificate();
            var validator = new X509CertificateChainValidator(rootCaCertificate);
            httpClientHandler.SslOptions.RemoteCertificateValidationCallback = validator.Validate;
        });

        return builder;
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
    /// Set what user data to supply to the auth request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authUserData"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseAuthRequestUserData(this IBankIdBuilder builder, BankIdAuthUserData authUserData)
    {
        builder.Services.AddTransient<IBankIdAuthRequestUserDataResolver>(x => new BankIdAuthRequestStaticUserDataResolver(authUserData));

        return builder;
    }

    /// <summary>
    /// Set what user data to supply to the auth request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authUserData"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseAuthRequestUserData(this IBankIdBuilder builder, Action<BankIdAuthUserData> authUserData)
    {
        var authUserDataResult = new BankIdAuthUserData();
        authUserData(authUserDataResult);
        UseAuthRequestUserData(builder, authUserDataResult);

        return builder;
    }

    /// <summary>
    /// Add a custom event listener.
    /// </summary>
    /// <typeparam name="TBankIdEventListenerImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddEventListener<TBankIdEventListenerImplementation>(this IBankIdBuilder builder) where TBankIdEventListenerImplementation : class, IBankIdEventListener
    {
        builder.Services.AddTransient<IBankIdEventListener, TBankIdEventListenerImplementation>();

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
    /// <typeparam name="TResultStoreImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddResultStore<TResultStoreImplementation>(this IBankIdBuilder builder) where TResultStoreImplementation : class, IBankIdResultStore
    {
        builder.Services.AddTransient<IBankIdResultStore, TResultStoreImplementation>();

        return builder;
    }

    /// <summary>
    /// Use a custom user data resolver.
    /// </summary>
    /// <typeparam name="TBankIdAuthRequestUserDataResolverImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseAuthRequestUserDataResolver<TBankIdAuthRequestUserDataResolverImplementation>(this IBankIdBuilder builder) where TBankIdAuthRequestUserDataResolverImplementation : class, IBankIdAuthRequestUserDataResolver
    {
        builder.Services.AddTransient<IBankIdAuthRequestUserDataResolver, TBankIdAuthRequestUserDataResolverImplementation>();

        return builder;
    }

    internal static IBankIdBuilder UseEnvironment(this IBankIdBuilder builder, Uri apiBaseUrl, string environment)
    {
        SetActiveLoginContext(builder.Services, environment, BankIdUrls.BankIdApiVersion);

        builder.ConfigureHttpClient(httpClient =>
        {
            httpClient.BaseAddress = apiBaseUrl;
        });

        builder.Services.AddTransient<IBankIdLauncher, BankIdLauncher>();

        if(builder is BankIdBuilder bankIdBuilder)
        {
            bankIdBuilder.AfterConfiguration();
        }

        return builder;
    }

    /// <summary>
    /// Use the BankID test environment (https://appapi2.test.bankid.com/rp/vX/)
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseTestEnvironment(this IBankIdBuilder builder)
    {
        return builder.UseEnvironment(BankIdUrls.TestApiBaseUrl, BankIdEnvironments.Test);
    }

    /// <summary>
    /// Use the BankID production environment (https://appapi2.bankid.com/rp/vX/)
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseProductionEnvironment(this IBankIdBuilder builder)
    {
        return builder.UseEnvironment(BankIdUrls.ProductionApiBaseUrl, BankIdEnvironments.Production);
    }

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder)
        => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient());

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="givenName">Fake given name</param>
    /// <param name="surname">Fake surname</param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname)
        => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname));

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="givenName">Fake given name</param>
    /// <param name="surname">Fake surname</param>
    /// <param name="personalIdentityNumber">Fake personal identity number</param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname, string personalIdentityNumber)
        => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname, personalIdentityNumber));

    private static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, Func<IServiceProvider, IBankIdApiClient> bankIdDevelopmentApiClient)
    {
        SetActiveLoginContext(builder.Services, BankIdEnvironments.Simulated, BankIdSimulatedApiClient.Version);

        builder.Services.AddSingleton(bankIdDevelopmentApiClient);
        builder.Services.AddSingleton<IBankIdLauncher, BankIdDevelopmentLauncher>();

        return builder;
    }

    private static void SetActiveLoginContext(IServiceCollection services, string environment, string apiVersion)
    {
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.BankIdApiEnvironment = environment;
            context.BankIdApiVersion = apiVersion;
        });
    }
}