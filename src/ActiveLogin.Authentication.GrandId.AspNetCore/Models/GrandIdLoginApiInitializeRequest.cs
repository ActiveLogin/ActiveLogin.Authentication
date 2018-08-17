﻿using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Identity.Swedish.AspNetCore.Validation;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    [DataContract]
    public class GrandIdLoginApiInitializeRequest
    {
        [DataMember(Name = "returnUrl")]
        public string ReturnUrl { get; set; }

        [DataMember(Name = "deviceOption")]
        public DeviceOption DeviceOption { get; set; } = DeviceOption.ChooseDevice;


    }
}