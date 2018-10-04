namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class AuthResponse
    {
        public AuthResponse()
        {
            
        }

        public AuthResponse(AuthFullResponse fullResponse)
        {
            RedirectUrl = fullResponse.RedirectUrl;
            SessionId = fullResponse.SessionId;
        }

        public string RedirectUrl { get; set; }
        public string SessionId { get; set; }
    }
}
