using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Auth response result.
    /// </summary>
    [DataContract]
    public class AuthResponse
    {
        internal AuthResponse(string orderRef, string autoStartToken)
        {
            OrderRef = orderRef;
            AutoStartToken = autoStartToken;
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
    }
}