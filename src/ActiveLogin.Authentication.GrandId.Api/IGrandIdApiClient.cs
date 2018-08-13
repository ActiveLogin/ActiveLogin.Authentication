using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// BankID API Client that defines the supported methods as defined in the document "BankID Relying Party Guidelines".
    /// </summary>
    public interface IGrandIdApiClient
    {
        /// <summary>
        /// Initiates an authentication or signing order. Use the collect method to query the status of the order.
        /// </summary>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        Task<AuthResponse> AuthAsync(AuthRequest request);

        /// <summary>
        /// Collects the result of a sign or auth order using the OrderRef as reference.
        /// RP should keep on calling collect every two seconds as long as status indicates pending.
        /// RP must abort if status indicates failed.
        /// </summary>
        /// <returns>The user identity is returned when complete.</returns>
        Task<CollectResponse> CollectAsync(CollectRequest request);

        /// <summary>
        /// Cancels an ongoing sign or auth order.
        /// This is typically used if the user cancels the order in your service or app.
        /// </summary>
        Task<CancelResponse> CancelAsync(CancelRequest request);
    }
}