using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow
{
    public interface IBankIdFlowService
    {
        string GetQrCode(BankIdQrStartState qrStartState);
        Task<InitializeAuthFlowResult> InitializeAuth(BankIdLoginOptions loginOptions, string returnRedirectUrl);

        Task<BankIdFlowCollectResult> TryCollect(string orderReference, int autoStartAttempts, BankIdLoginOptions loginOptions);
    }
}
