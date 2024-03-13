using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Phone auth response result.
/// </summary>
public abstract class PhoneResponse
{
    protected PhoneResponse(string orderRef)
    {
        OrderRef = orderRef;
    }

    /// <summary>
    /// Used to collect the status of the order.
    /// </summary>
    [JsonPropertyName("orderRef")]
    public string OrderRef { get; }
}
