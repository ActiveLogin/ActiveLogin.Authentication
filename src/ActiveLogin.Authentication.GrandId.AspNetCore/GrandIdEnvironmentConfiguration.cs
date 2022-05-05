namespace ActiveLogin.Authentication.GrandId.AspNetCore;

internal class GrandIdEnvironmentConfiguration : IGrandIdEnvironmentConfiguration
{
    /// <summary>
    /// The apiKey obtained from GrandID (Svensk E-identitet).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// The authenticateServiceKey for BankID obtained from GrandID (Svensk E-identitet).
    /// </summary>
    public string? BankIdServiceKey { get; set; } = null;
}