﻿using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class Error
    {
        internal Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        [DataMember(Name = "code")]
        public string Code { get; private set; }

        [DataMember(Name = "message")]
        public string Message { get; private set; }
    }
}