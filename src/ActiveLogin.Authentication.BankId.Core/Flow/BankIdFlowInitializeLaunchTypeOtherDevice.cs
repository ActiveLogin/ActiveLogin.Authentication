using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowInitializeLaunchTypeOtherDevice : BankIdFlowInitializeLaunchType
{
    public BankIdFlowInitializeLaunchTypeOtherDevice(BankIdQrStartState qrStartState, string qrCodeBase64Encoded)
    {
        QrStartState = qrStartState;
        QrCodeBase64Encoded = qrCodeBase64Encoded;
    }

    public BankIdQrStartState QrStartState { get; init; }

    public string QrCodeBase64Encoded { get; init; }
}
