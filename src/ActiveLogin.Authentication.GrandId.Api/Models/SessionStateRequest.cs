namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    //TODO: Make keys part of request
    public class SessionStateRequest
    {
        public SessionStateRequest(DeviceOption deviceOption, string sessionId)
        {
            DeviceOption = deviceOption;
            SessionId = sessionId;
        }
        public DeviceOption DeviceOption { get; set; }
        
        public string SessionId { get; set; }
    }
}