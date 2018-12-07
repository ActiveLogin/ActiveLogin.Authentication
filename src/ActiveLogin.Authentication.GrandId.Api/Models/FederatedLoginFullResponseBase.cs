using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public abstract class FederatedLoginFullResponseBase
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "errorObject")]
        public ErrorObject ErrorObject { get; set; }
    }
}