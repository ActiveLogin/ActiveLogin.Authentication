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
    Moderate
}
