﻿using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public abstract class SessionStateFullResponseBase<TUserAttributes> : FullResponseBase
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "userAttributes")]
        public TUserAttributes UserAttributes { get; set; }
    }
}