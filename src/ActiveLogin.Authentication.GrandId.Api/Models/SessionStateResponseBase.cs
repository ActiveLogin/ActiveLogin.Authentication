namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class SessionStateResponseBase
    {
        internal SessionStateResponseBase(SessionStateFullResponseBase fullResponse)
        {
            SessionId = fullResponse.SessionId;
            Username = fullResponse.UserName;
        }

        protected SessionStateResponseBase(string sessionId, string username)
        {
            SessionId = sessionId;
            Username = username;
        }

        public string SessionId { get; }
        public string Username { get; }
    }
}