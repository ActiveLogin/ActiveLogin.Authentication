using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Cancel request parameters.
    /// </summary>
    public class CancelRequest
    {
        /// <summary></summary>
        /// <param name="orderRef">
        /// The OrderRef from the response from auth or sign.
        /// </param>
        public CancelRequest(string orderRef)
        {
            OrderRef = orderRef;
        }

        /// <summary>
        /// The OrderRef from the response from auth or sign.
        /// </summary>
        [JsonPropertyName("orderRef")]
        public string OrderRef { get; private set; }
    }
}
