using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class LogoutFullResponse : FullResponseBase
    {
        [DataMember(Name = "sessiondeleted")]
        public string SessionDeleted { get; set; }
    }
}