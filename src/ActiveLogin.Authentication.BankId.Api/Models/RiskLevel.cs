namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Represents the risk levels returned in the collect response.
/// </summary>
public enum RiskLevel
{
    /// <summary>
    /// If no value was returned the risk couldn’t be calculated.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Low risk orders. No or low risk identified in the available order data.
    /// </summary>
    Low,

    /// <summary>
    /// Moderate risk orders. Might need further action, investigation or follow-up by the Relying Party based on order data.
    /// </summary>
    Moderate,

    /// <summary>
    /// High risk transaction. The order should be blocked/cancelled by the Relying Party and needs further action, investigation or follow-up.
    /// </summary>
    High
}
