namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class LogoutResponse
    {
        public LogoutResponse()
        {
            
        }

        public LogoutResponse(LogoutFullResponse fullResponse)
        {
            SessionDeleted = fullResponse.SessionDeleted?.Equals("1") ?? false;
        }

        public bool SessionDeleted { get; set; }
    }
}
