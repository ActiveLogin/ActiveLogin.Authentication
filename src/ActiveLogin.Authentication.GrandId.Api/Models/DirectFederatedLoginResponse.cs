namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class DirectFederatedLoginResponse : FederatedLoginResponseBase<DirectFederatedLoginFullResponse>
    {
        public DirectFederatedLoginResponse()
        {

        }

        public DirectFederatedLoginResponse(DirectFederatedLoginFullResponse fullResponse)
            : base(fullResponse)
        {
            Username = fullResponse.Username;
            UserAttributes = fullResponse.UserAttributes;
        }

        public string Username { get; set; }

        public DirectFederatedLoginUserAttributes UserAttributes { get; set; }
    }
}
