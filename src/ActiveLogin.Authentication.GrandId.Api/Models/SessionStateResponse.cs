using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class SessionStateResponse
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "userAttributes")]
        public UserAttributes UserAttributes { get; set; }
    }
}