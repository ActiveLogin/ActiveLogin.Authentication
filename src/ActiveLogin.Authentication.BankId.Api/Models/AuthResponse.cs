using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Auth response result.
    /// </summary>
    [DataContract]
    public class AuthResponse
    {
        public AuthResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret)
        {
            OrderRef = orderRef;
            AutoStartToken = autoStartToken;
            QrStartToken = qrStartToken;
            QrStartSecret = qrStartSecret;
        }

        /// <summary>
        /// Used to collect the status of the order.
        /// </summary>
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; private set; }

        /// <summary>
        /// Used as reference to this order when the client is started automatically.
        /// </summary>
        [DataMember(Name = "autoStartToken")]
        public string AutoStartToken { get; private set; }

        /// <summary>
        /// Used to compute the animated QR code.
        /// </summary>
        [DataMember(Name = "qrStartToken")]
        public string QrStartToken { get; private set; }

        /// <summary>
        /// Used to compute the animated QR code.
        /// </summary>
        [DataMember(Name = "qrStartSecret")]
        public string QrStartSecret { get; private set; }
    }
}
