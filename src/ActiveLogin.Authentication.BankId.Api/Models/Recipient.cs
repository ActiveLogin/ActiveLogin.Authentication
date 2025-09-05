using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// The recipient of the payment.
/// </summary>
public class Recipient
{
    /// <summary></summary>
    /// <param name="name">The name of the recipient of the payment. For the transaction type "card", this is the merchant name.</param>
    public Recipient(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The name of the recipient of the payment. For the transaction type "card", this is the merchant name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }
}
