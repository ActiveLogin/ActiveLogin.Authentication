namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedDirectLoginRequest
    {
        public FederatedDirectLoginRequest(string authenticateServiceKey, string username, string password)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            Username = username;
            Password = password;
        }

        public string AuthenticateServiceKey { get; }
        public string Username { get; }
        public string Password { get; }
    }
}