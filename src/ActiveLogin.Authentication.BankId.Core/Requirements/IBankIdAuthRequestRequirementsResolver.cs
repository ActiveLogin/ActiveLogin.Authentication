namespace ActiveLogin.Authentication.BankId.Core.Requirements;

/// <summary>
/// Resolve auth request requirements.
/// </summary>
public interface IBankIdAuthRequestRequirementsResolver
{
    /// <summary>
    /// Returns the requirements for the current request.
    /// </summary>
    /// <returns></returns>
    Task<BankIdAuthRequirements> GetRequirementsAsync();
}
