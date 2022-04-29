using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow
{
    public interface IBankIdFlowService
    {
        Task<BankIdFlowInitializeAuthResult> InitializeAuth(BankIdLoginOptions loginOptions, string returnRedirectUrl);

        Task<BankIdFlowCollectResult> Collect(string orderRef, int autoStartAttempts, BankIdLoginOptions loginOptions);

        Task Cancel(string orderRef, BankIdLoginOptions loginOptions);

        string GetQrCodeAsBase64(BankIdQrStartState qrStartState);
    }
}
