using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class AuthFullResponse
    {
        [DataMember(Name = "redirectUrl")]
        public string RedirectUrl { get; set; }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "errorObject")]
        public ErrorObject ErrorObject { get; set; }
    }
}