namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class SessionStateResponse
    {
        public SessionStateResponse()
        {
            
        }

        public SessionStateResponse(SessionStateFullResponse fullResponse)
        {
            SessionId = fullResponse.SessionId;
            UserName = fullResponse.UserName;
            UserAttributes = fullResponse.UserAttributes;
        }

        public string SessionId { get; set; }
        public string UserName { get; set; }
        public UserAttributes UserAttributes { get; set; }
    }
}