using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    internal class BankIdGetSessionFullResponse : GetSessionFullResponseBase
    {
        [DataMember(Name = "userAttributes")]
        public BankIdGetSessionUserAttributes UserAttributes { get; private set; }
    }
}
