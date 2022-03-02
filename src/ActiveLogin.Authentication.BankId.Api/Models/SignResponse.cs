using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Sign response result.
    /// </summary>
    public class SignResponse
    {
        public SignResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret)
        {
            OrderRef = orderRef;
            AutoStartToken = autoStartToken;
            QrStartToken = qrStartToken;
            QrStartSecret = qrStartSecret;
        }

        /// <summary>
        /// Used to collect the status of the order.
        /// </summary>
        [JsonPropertyName("orderRef")]
        public string OrderRef { get; private set; }

        /// <summary>
        /// Used as reference to this order when the client is started automatically.
        /// </summary>
        [JsonPropertyName("autoStartToken")]
        public string AutoStartToken { get; private set; }

        /// <summary>
        /// Used to compute the animated QR code.
        /// </summary>
        [JsonPropertyName("qrStartToken")]
        public string QrStartToken { get; private set; }

        /// <summary>
        /// Used to compute the animated QR code.
        /// </summary>
        [JsonPropertyName("qrStartSecret")]
        public string QrStartSecret { get; private set; }
    }
}
