namespace ActiveLogin.Authentication.BankId.Core.Risk;

public enum BankIdAllowedRiskLevel
{
    /// <summary>
    /// Only accept low risk orders.
    /// </summary>
    Low,

    /// <summary>
    /// Accept low and moderate risk orders
    /// </summary>
    Moderate,

    /// <summary>
    /// No risk level is set and the BankID service will not block any order regardless of the risk indication.
    /// </summary>
    NoRiskLevel
}
