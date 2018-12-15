﻿using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class Error
    {
        [DataMember(Name = "errorObject")]
        private string ErrorObject { get; set; }

        public ErrorCode ErrorCode
        {
            get
            {
                Enum.TryParse<ErrorCode>(ErrorObject, true, out var parsedErrorCode);
                return parsedErrorCode;
            }
        }

        [DataMember(Name = "message")]
        public string Details { get; set; }
    }
}
