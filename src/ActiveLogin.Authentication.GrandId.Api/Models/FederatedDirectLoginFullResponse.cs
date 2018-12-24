using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    internal class FederatedDirectLoginFullResponse : FullResponseBase
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; private set; }

        [DataMember(Name = "username")]
        public string Username { get; private set; }

        [DataMember(Name = "userAttributes")]
        public FederatedDirectLoginUserAttributes UserAttributes { get; private set; }
    }
}