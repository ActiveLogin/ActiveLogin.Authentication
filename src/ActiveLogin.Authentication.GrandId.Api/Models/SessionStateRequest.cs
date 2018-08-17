using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class SessionStateRequest
    {
        public SessionStateRequest(string apiKey, string authenticateServiceKey, string sessionId)
        {
            ApiKey = apiKey;
            AuthenticateServiceKey = authenticateServiceKey;
            SessionId = sessionId;
        }

        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        [DataMember(Name = "authenticateServiceKey")]
        public string AuthenticateServiceKey { get; set; }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}