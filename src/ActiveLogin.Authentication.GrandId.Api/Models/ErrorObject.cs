﻿using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class ErrorObject
    {

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}