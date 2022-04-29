using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.Core;

/// <summary>
/// Resolve auth request user data.
/// </summary>
public interface IBankIdAuthRequestUserDataResolver
{
    /// <summary>
    /// Returns the user data for the current context/request.
    /// </summary>
    /// <param name="authRequestContext">BankID auth request context.</param>
    /// <returns></returns>
    Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext);
}
