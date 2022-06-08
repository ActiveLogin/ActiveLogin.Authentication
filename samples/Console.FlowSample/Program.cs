using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserData;
using ActiveLogin.Authentication.BankId.Core.UserMessage;
using ActiveLogin.Authentication.BankId.QrCoder;

using Console.FlowSample;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    .ConfigureServices((_, services) =>
    {
        services.AddBankId(bankId =>
        {
            bankId.UseSimulatedEnvironment();
            bankId.UseQrCoderQrCodeGenerator();
        });

        services.AddTransient<IBankIdAuthRequestUserDataResolver, BankIdAuthRequestEmptyUserDataResolver>();
        services.AddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetectorUnknown>();
        services.AddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();

        services.AddTransient<IBankIdEndUserIpResolver, BankIdLocalIpAddressEndUserIpResolver>();

        services.AddHostedService<BankIdDemoHostedService>();
    })
    .Build();

await host.RunAsync();
