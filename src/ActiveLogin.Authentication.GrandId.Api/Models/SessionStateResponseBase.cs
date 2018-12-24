namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class SessionStateResponseBase<TFullResponse, TUserAttributes> where TFullResponse : SessionStateFullResponseBase<TUserAttributes>
    {
        protected SessionStateResponseBase()
        {
            
        }

        protected SessionStateResponseBase(TFullResponse fullResponse)
        {
            SessionId = fullResponse.SessionId;
            Username = fullResponse.UserName;
            UserAttributes = fullResponse.UserAttributes;
        }

        public string SessionId { get; set; }
        public string Username { get; set; }
        public TUserAttributes UserAttributes { get; set; }
    }
}