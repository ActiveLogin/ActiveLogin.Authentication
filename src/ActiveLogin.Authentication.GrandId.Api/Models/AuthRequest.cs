using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class AuthRequest
    {

        public AuthRequest(DeviceOption deviceOption, string callbackUrl)
        {
            DeviceOption = deviceOption;
            CallbackUrl = callbackUrl;
        }

        [DataMember(Name = "deviceOption")]
        public DeviceOption DeviceOption { get; set; }

        [DataMember(Name = "callbackUrl")]
        public string CallbackUrl { get; set; }
    }
}