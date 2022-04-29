using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow;

public class BankIdFlowInitializeAuthLaunchTypeOtherDevice : BankIdFlowInitializeAuthLaunchType
{
    public BankIdFlowInitializeAuthLaunchTypeOtherDevice(BankIdQrStartState qrStartState, string qrCodeBase64Encoded)
    {
        QrStartState = qrStartState;
        QrCodeBase64Encoded = qrCodeBase64Encoded;
    }

    public BankIdQrStartState QrStartState { get; init; }

    public string QrCodeBase64Encoded { get; init; }
}
