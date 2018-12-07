using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class DirectFederatedLoginFullResponse : FederatedLoginFullResponseBase
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "userAttributes")]
        public DirectFederatedLoginUserAttributes UserAttributes { get; set; }
    }
}