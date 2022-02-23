using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Collect request parameters.
    /// </summary>
    public class CollectRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderRef">
        /// The OrderRef returned from auth or sign.
        /// </param>
      
        public CollectRequest(string orderRef)
        {
            OrderRef = orderRef;
        }

        /// <summary>
        /// The OrderRef returned from auth or sign.
        /// </summary>
        [JsonPropertyName("orderRef")]
        public string OrderRef { get; private set; }
    }
}
