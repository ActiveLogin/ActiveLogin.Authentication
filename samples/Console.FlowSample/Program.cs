using ActiveLogin.Authentication.BankId.AzureKeyVault;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserData;
using ActiveLogin.Authentication.BankId.QrCoder;

using Console.FlowSample;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

//
// DISCLAIMER - DO NOT USE FOR REAL
//
// This is samples to show how the BankID flow works.
// You can't use BankID in this way in an application for real
// as the client certificates would be exposed.
//
// Please see this as technical demo of how the flow works,
// not something to use.
//

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddUserSecrets(typeof(Program).Assembly);
    })
    .ConfigureLogging(context =>
    {
        context.ClearProviders();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddBankId(bankId =>
        {
            bankId.UseQrCoderQrCodeGenerator();

            if (configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
            {
                bankId.UseSimulatedEnvironment();
            }
            else if (configuration.GetValue("ActiveLogin:BankId:UseTestEnvironment", false))
            {
                bankId.UseTestEnvironment();
            }
            else
            {
                bankId.UseProductionEnvironment();
                bankId.UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
            }
        });

        services.AddTransient<IBankIdAuthRequestUserDataResolver, BankIdAuthRequestEmptyUserDataResolver>();
        services.AddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetectorUnknown>();

        services.AddTransient<IBankIdEndUserIpResolver, BankIdLocalIpAddressEndUserIpResolver>();
        services.AddTransient<IBankIdQrCodeGenerator, QrCoderAsciiBankIdQrCodeGenerator>();

        services.AddHostedService<BankIdDemoHostedService>();
    })
    .Build();

await host.RunAsync();
