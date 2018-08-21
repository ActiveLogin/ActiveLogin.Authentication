using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    [DataContract]
    public class SessionStateRequest
    {
        public SessionStateRequest(DeviceOption deviceOption, string sessionId)
        {
            DeviceOption = deviceOption;
            SessionId = sessionId;
        }
        [DataMember(Name = "deviceOption")]
        public DeviceOption DeviceOption { get; set; }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}