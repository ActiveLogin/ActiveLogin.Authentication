using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

public class PhoneAuthResponse
{
    public PhoneAuthResponse(string orderRef)
    {
        OrderRef = orderRef;
    }

    /// <summary>
    /// Used to collect the status of the order.
    /// </summary>
    [JsonPropertyName("orderRef")]
    public string OrderRef { get; }
}
