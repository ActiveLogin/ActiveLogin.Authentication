using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class AuthResponse
    {
        [DataMember(Name = "orderRef")]
        public string OrderRef { get; set; }

        [DataMember(Name = "autoStartToken")]
        public string AutoStartToken { get; set; }
    }
}
