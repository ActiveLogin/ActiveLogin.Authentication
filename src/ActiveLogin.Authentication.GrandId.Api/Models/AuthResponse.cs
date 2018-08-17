using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class AuthResponse
    {
        [DataMember(Name = "redirectUrl")]
        public string RedirectUrl { get; set; }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}
