using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models;

[DataContract]
internal class LogoutFullResponse : FullResponseBase
{
    [DataMember(Name = "sessiondeleted")]
    public string? SessionDeleted { get; private set; }
}