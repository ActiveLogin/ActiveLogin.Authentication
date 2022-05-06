using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowInitializeAuthResult
{
    public BankIdFlowInitializeAuthResult(AuthResponse bankIdAuthResponse, BankIdSupportedDevice detectedUserDevice, BankIdFlowInitializeAuthLaunchType launchType)
    {
        BankIdAuthResponse = bankIdAuthResponse;
        DetectedUserDevice = detectedUserDevice;
        LaunchType = launchType;
    }

    public AuthResponse BankIdAuthResponse { get; init; }

    public BankIdSupportedDevice DetectedUserDevice { get; init; }

    public BankIdFlowInitializeAuthLaunchType LaunchType { get; init; }
}
