using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Identity.Swedish;

using Microsoft.Extensions.Hosting;

namespace Phone.ConsoleSample;

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

internal sealed class BankIdDemoHostedService : IHostedService
{
    private readonly IBankIdAppApiClient _bankIdApiClient;
    private readonly IHostApplicationLifetime _appLifetime;

    public BankIdDemoHostedService(
        IBankIdAppApiClient bankIdApiClient,
        IHostApplicationLifetime appLifetime
    )
    {
        _bankIdApiClient = bankIdApiClient;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await RunBankIdFlowAsync();
        _appLifetime.StopApplication();

    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task RunBankIdFlowAsync()
    {
        var personalIdentityNumber = GetPersonalIdentityNumber();
        var sessionType = GetSessionType();
        var callInitiatior = GetCallInitiator();

        var orderRef = await InitiateAsync(personalIdentityNumber, sessionType, callInitiatior);
        await CollectAsync(orderRef);
    }

    private string GetPersonalIdentityNumber()
    {
        ConsoleHelper.WriteHeader("Enter the personal identity number (YYYYMMDDXXXX) of the one:");
        while (true)
        {
            var personalIdentityNumber = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(personalIdentityNumber))
            {
                var success = PersonalIdentityNumber.TryParse(personalIdentityNumber, out var parsedPersonalIdentityNumber);
                if (success)
                {
                    Console.WriteLine();
                    return parsedPersonalIdentityNumber.To12DigitString();
                }
                else
                {
                    Console.WriteLine("Invalid personal identity number. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
            }
        }
    }

    private SessionType GetSessionType()
    {
        ConsoleHelper.WriteHeader("Do you want to test an auth or sign session?");
        var sessionType = ConsoleHelper.DisplayMenuAndGetSelectedKey(new List<(string Key, string DisplayName)>
        {
            (SessionType.Auth.ToString(), "Auth"),
            (SessionType.Sign.ToString(), "Sign")
        });
        return Enum.Parse<SessionType>(sessionType);
    }

    private CallInitiator GetCallInitiator()
    {
        ConsoleHelper.WriteHeader("Who is the initiator of the session?");
        var callInitiatior = ConsoleHelper.DisplayMenuAndGetSelectedKey(new List<(string Key, string DisplayName)>
        {
            (CallInitiator.RP.ToString(), $"{CallInitiator.RP} - user called the RP"),
            (CallInitiator.User.ToString(), $"{CallInitiator.User} - RP called the user")
        });
        return Enum.Parse<CallInitiator>(callInitiatior);
    }

    private async Task<string> InitiateAsync(string personalIdentityNumber, SessionType sessionType, CallInitiator callInitiator)
    {
        ConsoleHelper.WriteHeader($"Initiates a {sessionType} session");
        var orderRef = "";
        if (sessionType == SessionType.Auth)
        {
            var response = await _bankIdApiClient.PhoneAuthAsync(new PhoneAuthRequest(
                personalIdentityNumber: personalIdentityNumber,
                callInitiator: callInitiator,
                requirement: null,
                userVisibleData: null,
                userNonVisibleData: null,
                userVisibleDataFormat: null
            ));
            orderRef = response.OrderRef;
        }
        else
        {
            var response = await _bankIdApiClient.PhoneSignAsync(new PhoneSignRequest(
                personalIdentityNumber: personalIdentityNumber,
                callInitiator: callInitiator,
                requirement: null,
                userVisibleData: "Hello, this is just a sample",
                userNonVisibleData: null,
                userVisibleDataFormat: null
            ));
            orderRef = response.OrderRef;
        }
        Console.WriteLine($"Successfully initiated a session with orderRef: {orderRef}");
        Console.WriteLine();
        return orderRef;
    }

    private async Task CollectAsync(string orderRef)
    {
        ConsoleHelper.WriteHeader($"Collecte status for orderRef: {orderRef}");
        while (true)
        {
            var collectResponse = await _bankIdApiClient.CollectAsync(new CollectRequest(orderRef));
            var status = collectResponse.GetCollectStatus();
            if (status == CollectStatus.Pending)
            {
                Console.WriteLine($"Pending. HintCode: {collectResponse.HintCode}");
                await Task.Delay(2000);
            }
            else if (status == CollectStatus.Complete)
            {
                Console.WriteLine("");
                ConsoleHelper.WriteHeader("Collect completed");
                Console.WriteLine($"Name: {collectResponse.CompletionData!.User.Name}");
                Console.WriteLine($"Ip-Adress: {collectResponse.CompletionData!.Device.IpAddress}");
                break;
            }
            else
            {
                Console.WriteLine("");
                ConsoleHelper.WriteHeader($"Failed. HintCode: {collectResponse.HintCode}");
                break;
            }
        }
    }
    
}
