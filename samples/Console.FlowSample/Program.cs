using ActiveLogin.Authentication.BankId.Core;

using Microsoft.Extensions.Hosting;

//
// DISCLAIMER - DO NOT USE FOR REAL
//
// This is samples to show how the BankID flow works.
// You can't use BankID in this way in an application for real
// as the client certificates would be exposed.
//
// Please see this as inspiration, not something to use.
//

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddBankId(bankId =>
        {
            
        });
    })
    .Build();


await host.RunAsync();
