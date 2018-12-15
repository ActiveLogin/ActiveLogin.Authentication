﻿using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public abstract class FederatedLoginFullResponseBase : FullResponseBase
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "redirectUrl")]
        public string RedirectUrl { get; set; }
    }
}