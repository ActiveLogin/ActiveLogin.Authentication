namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class SessionStateResponseBase
    {
        protected SessionStateResponseBase()
        {
            
        }

        internal SessionStateResponseBase(SessionStateFullResponseBase fullResponse)
        {
            SessionId = fullResponse.SessionId;
            Username = fullResponse.UserName;
        }

        public string SessionId { get; set; }
        public string Username { get; set; }
    }
}