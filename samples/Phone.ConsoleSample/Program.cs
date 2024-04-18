using ActiveLogin.Authentication.BankId.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ActiveLogin.Authentication.BankId.AzureKeyVault;
using Phone.ConsoleSample;

//
// DISCLAIMER - DO NOT USE FOR REAL
//
// This is samples to show how the BankID phone flow works.
// You can't use BankID in this way in an application for real
// as the client certificates would be exposed.
//
// Please see this as technical demo of how the flow works,
// not something to use. In a production scenario you should
// integrate this into a backoffice system.
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
        
        services.AddHostedService<BankIdDemoHostedService>();
    })
    .Build();

await host.RunAsync();
