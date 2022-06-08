using ActiveLogin.Authentication.BankId.Api.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;

using Spectre.Console;

namespace Console.FlowSample;

internal sealed class BankIdDemoHostedService : IHostedService
{
    private readonly IBankIdFlowService _bankIdFlowService;

    public BankIdDemoHostedService(IBankIdFlowService bankIdFlowService)
    {
        _bankIdFlowService = bankIdFlowService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var policies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(BankIdCertificatePolicy.MobileBankId);
        var flowOptions = new BankIdFlowOptions(policies, false, true);
        var initializeResult = await _bankIdFlowService.InitializeAuth(flowOptions, "/");
        var launchType = initializeResult.LaunchType as BankIdFlowInitializeLaunchTypeOtherDevice ?? throw new InvalidOperationException();
        var isCompleted = false;

        var imageBytes = Convert.FromBase64String(launchType.QrCodeBase64Encoded);
        var image = new CanvasImage(imageBytes);
        image.MaxWidth(55);
        image.PixelWidth = 1;
        AnsiConsole.Write(image);

        while (!isCompleted)
        {
            var collectResult = await _bankIdFlowService.Collect(initializeResult.BankIdResponse.OrderRef, 0, flowOptions);

            if (collectResult is BankIdFlowCollectResultComplete collectResultComplete)
            {
                System.Console.WriteLine(collectResultComplete.CompletionData.User.Name);
                isCompleted = true;
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
