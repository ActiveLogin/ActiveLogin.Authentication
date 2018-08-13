using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    /// <summary>
    /// Auth response result.
    /// </summary>
    [DataContract]
    public class AuthResponse
    {
        /// <summary>
        /// Used to collect the status of the order.
        /// </summary>
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; set; }

        /// <summary>
        /// Used as reference to this order when the client is started automatically.
        /// </summary>
        [DataMember(Name = "autoStartToken")]
        public string AutoStartToken { get; set; }
    }
}
