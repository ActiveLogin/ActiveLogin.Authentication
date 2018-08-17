﻿using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class AuthRequest
    {

        public AuthRequest(string apiKey, string authenticateServiceKey, string callbackUrl)
        {
            ApiKey = apiKey;
            AuthenticateServiceKey = authenticateServiceKey;
            CallbackUrl = callbackUrl;
        }

        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        [DataMember(Name = "authenticateServiceKey")]
        public string AuthenticateServiceKey { get; set; }

        [DataMember(Name = "callbackUrl")]
        public string CallbackUrl { get; set; }
    }
}