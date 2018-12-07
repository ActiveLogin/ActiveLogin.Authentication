using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class BankIdFederatedLoginFullResponse : FederatedLoginFullResponseBase
    {
        [DataMember(Name = "redirectUrl")]
        public string RedirectUrl { get; set; }
    }
}