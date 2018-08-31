namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    //TODO: Make keys part of request
    public class AuthRequest
    {
        public AuthRequest(DeviceOption deviceOption, string callbackUrl)
        {
            DeviceOption = deviceOption;
            CallbackUrl = callbackUrl;
        }

        public DeviceOption DeviceOption { get; set; }

        public string CallbackUrl { get; set; }
    }
}