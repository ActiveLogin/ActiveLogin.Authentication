using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class LogoutFullResponse
    {
        [DataMember(Name = "sessiondeleted")]
        public string SessionDeleted { get; set; }

        [DataMember(Name = "errorObject")]
        public ErrorObject ErrorObject { get; set; }
    }
}