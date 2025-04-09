using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Information about the transaction being approved.
/// </summary>
public class UserVisibleTransaction
{
    /// <param name="transactionType">
    /// The type of transaction.
    /// The possible values have the following meaning:
    /// card: Card payment.
    /// npa: Non-payment authentication.
    /// </param>
    /// <param name="recipient">
    /// The recipient of the payment.
    /// </param>
    /// <param name="money">
    /// Object that sets monetary amount for the payment.
    /// If the transactionType is npa this isn't allowed to be set.
    /// </param>
    /// <param name="riskWarning">
    /// Indicate to the user that the payment has higher risk or is unusual for the user.
    /// This will be indicated in the UI.
    /// </param>
    public UserVisibleTransaction(string transactionType, Recipient recipient, Money? money = null, string? riskWarning = null)
    {
        TransactionType = transactionType;
        Recipient = recipient;
        Money = money;
        RiskWarning = riskWarning;
    }

    /// <summary>
    /// The type of transaction.
    /// The possible values have the following meaning:
    /// card: Card payment.
    /// npa: Non-payment authentication.
    /// </summary>
    [JsonPropertyName("transactionType")]
    public string TransactionType { get; }

    /// <summary>
    /// The recipient of the payment.
    /// </summary>
    [JsonPropertyName("recipient")]
    public Recipient Recipient { get; }

    /// <summary>
    /// Object that sets monetary amount for the payment.
    /// If the transactionType is npa this isn't allowed to be set.
    /// </summary>
    [JsonPropertyName("money"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Money? Money { get; }

    /// <summary>
    /// Indicate to the user that the payment has higher risk or is unusual for the user.
    /// This will be indicated in the UI.
    /// </summary>
    [JsonPropertyName("riskWarning"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RiskWarning { get; }
}
