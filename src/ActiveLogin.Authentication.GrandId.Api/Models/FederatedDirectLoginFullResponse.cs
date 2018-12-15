using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class FederatedDirectLoginFullResponse : FullResponseBase
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "userAttributes")]
        public FederatedDirectLoginUserAttributes UserAttributes { get; set; }
    }
}