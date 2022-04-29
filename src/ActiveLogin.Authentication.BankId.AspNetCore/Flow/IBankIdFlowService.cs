using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow
{
    public interface IBankIdFlowService
    {
        Task<InitializeAuthFlowResult> InitializeAuth(BankIdLoginOptions loginOptions, string returnRedirectUrl);
        string GetQrCode(BankIdQrStartState qrStartState);
    }
}
