using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models;

[DataContract]
internal abstract class FederatedLoginFullResponseBase : FullResponseBase
{
    [DataMember(Name = "sessionId")]
    public string? SessionId { get; private set; }

    [DataMember(Name = "redirectUrl")]
    public string? RedirectUrl { get; private set; }
}