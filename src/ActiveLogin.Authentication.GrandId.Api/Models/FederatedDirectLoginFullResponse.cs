using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class FederatedDirectLoginFullResponse
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "userAttributes")]
        public FederatedDirectLoginUserAttributes UserAttributes { get; set; }

        [DataMember(Name = "errorObject")]
        public ErrorObject ErrorObject { get; set; }
    }
}