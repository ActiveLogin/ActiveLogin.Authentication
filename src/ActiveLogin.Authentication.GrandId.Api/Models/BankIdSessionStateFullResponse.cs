using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    internal class BankIdSessionStateFullResponse : SessionStateFullResponseBase
    {
        [DataMember(Name = "userAttributes")]
        public BankIdSessionStateUserAttributes UserAttributes { get; private set; }
    }
}