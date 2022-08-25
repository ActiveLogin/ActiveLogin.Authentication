using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowInitializeResult
{
    public BankIdFlowInitializeResult(Response bankIdResponse, BankIdSupportedDevice detectedUserDevice, BankIdFlowInitializeLaunchType launchType)
    {
        BankIdResponse = bankIdResponse;
        DetectedUserDevice = detectedUserDevice;
        LaunchType = launchType;
    }

    public Response BankIdResponse { get; init; }

    public BankIdSupportedDevice DetectedUserDevice { get; init; }

    public BankIdFlowInitializeLaunchType LaunchType { get; init; }
}
