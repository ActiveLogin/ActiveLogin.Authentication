using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Collect request parameters.
    /// </summary>
    [DataContract]
    public class CollectRequest
    {
        public CollectRequest(string orderRef)
        {
            OrderRef = orderRef;
        }

        /// <summary>
        /// The OrderRef returned from auth or sign.
        /// </summary>
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; set; }
    }
}