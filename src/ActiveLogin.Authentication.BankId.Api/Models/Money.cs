using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Object that sets monetary amount for the payment.
/// If the transactionType is npa this isn't allowed to be set.
/// </summary>
public class Money
{
    /// <summary></summary>
    /// <param name="amount">
    /// The monetary amount of the payment.
    /// The string can contain one decimal separator which must be ",". The rest of the input must be numbers.
    /// Examples: "1000,00", "100,000", "100", "0"
    /// </param>
    /// <param name="currency">
    /// The currency of the payment.
    /// This must be an ISO 4217 alphabetic currency code.
    /// </param>
    public Money(string amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// The monetary amount of the payment.
    /// The string can contain one decimal separator which must be ",". The rest of the input must be numbers.
    /// Examples: "1000,00", "100,000", "100", "0"
    /// </summary>
    [JsonPropertyName("amount")]
    public string Amount { get; }

    /// <summary>
    /// The currency of the payment.
    /// This must be an ISO 4217 alphabetic currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; }
}
