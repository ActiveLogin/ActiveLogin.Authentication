using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class CollectRequest
    {
        public CollectRequest(string orderRef)
        {
            OrderRef = orderRef;
        }

        [DataMember(Name = "orderRef")]
        public string OrderRef { get; set; }
    }
}