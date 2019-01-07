using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    internal abstract class FullResponseBase
    {
        [DataMember(Name = "errorObject")]
        public Error ErrorObject { get; private set; }
    }
}