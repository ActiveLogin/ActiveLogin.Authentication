namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class GetSessionResponseBase
    {
        internal GetSessionResponseBase(GetSessionFullResponseBase fullResponse)
        {
            SessionId = fullResponse.SessionId;
            Username = fullResponse.UserName;
        }

        internal GetSessionResponseBase(string sessionId, string username)
        {
            SessionId = sessionId;
            Username = username;
        }

        public string SessionId { get; }
        public string Username { get; }
    }
}
