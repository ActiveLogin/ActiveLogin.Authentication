using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    public interface IBankIdApiClient
    {
        Task<AuthResponse> AuthAsync(AuthRequest request);
        Task<CollectResponse> CollectAsync(CollectRequest request);
        Task<CancelResponse> CancelAsync(CancelRequest request);
    }
}