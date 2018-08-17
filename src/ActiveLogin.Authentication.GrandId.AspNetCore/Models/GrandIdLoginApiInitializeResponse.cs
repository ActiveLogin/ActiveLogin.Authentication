using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    [DataContract]
    public class GrandIdLoginApiInitializeResponse
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "redirectUrl")]
        public string RedirectUrl { get; set; }
    }
}