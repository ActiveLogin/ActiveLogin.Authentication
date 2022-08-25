using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models;

[DataContract]
internal abstract class GetSessionFullResponseBase : FullResponseBase
{
    [DataMember(Name = "sessionId")]
    public string? SessionId { get; private set; }

    [DataMember(Name = "username")]
    public string? UserName { get; private set; }
}