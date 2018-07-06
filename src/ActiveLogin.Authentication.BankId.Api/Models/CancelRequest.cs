using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class CancelRequest
    {
        public CancelRequest(string orderRef)
        {
            OrderRef = orderRef;
        }

        [DataMember(Name = "orderRef")]
        public string OrderRef { get; set; }
    }
}
