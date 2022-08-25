using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public interface IBankIdFlowService
{
    Task<BankIdFlowInitializeResult> InitializeAuth(BankIdFlowOptions flowOptions, string returnRedirectUrl);

    Task<BankIdFlowInitializeResult> InitializeSign(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, string returnRedirectUrl);

    Task<BankIdFlowCollectResult> Collect(string orderRef, int autoStartAttempts, BankIdFlowOptions flowOptions);

    Task Cancel(string orderRef, BankIdFlowOptions flowOptions);

    string GetQrCodeAsBase64(BankIdQrStartState qrStartState);
}
