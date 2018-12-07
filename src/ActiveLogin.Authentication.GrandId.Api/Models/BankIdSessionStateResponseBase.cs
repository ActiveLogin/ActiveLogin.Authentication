namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class BankIdSessionStateResponseBase<TFullResponse, TUserAttributes> where TFullResponse : SessionStateFullResponseBase<TUserAttributes>
    {
        protected BankIdSessionStateResponseBase()
        {
            
        }

        protected BankIdSessionStateResponseBase(TFullResponse fullResponse)
        {
            SessionId = fullResponse.SessionId;
            UserName = fullResponse.UserName;
            UserAttributes = fullResponse.UserAttributes;
        }

        public string SessionId { get; set; }
        public string UserName { get; set; }
        public TUserAttributes UserAttributes { get; set; }
    }
}