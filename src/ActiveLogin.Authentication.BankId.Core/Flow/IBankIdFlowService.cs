using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public interface IBankIdFlowService
{
    Task<BankIdFlowInitializeAuthResult> InitializeAuth(BankIdFlowOptions flowOptions, string returnRedirectUrl);

    Task<BankIdFlowCollectResult> Collect(string orderRef, int autoStartAttempts, BankIdFlowOptions flowOptions);

    Task Cancel(string orderRef, BankIdFlowOptions flowOptions);

    string GetQrCodeAsBase64(BankIdQrStartState qrStartState);
}
