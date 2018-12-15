using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public abstract class FullResponseBase
    {
        [DataMember(Name = "errorObject")]
        public ErrorObject ErrorObject { get; set; }
    }
}