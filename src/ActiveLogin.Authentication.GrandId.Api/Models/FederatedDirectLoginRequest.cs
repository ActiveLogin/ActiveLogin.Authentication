namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedDirectLoginRequest
    {
        public FederatedDirectLoginRequest()
        {
            
        }

        public FederatedDirectLoginRequest(string authenticateServiceKey, string username, string password)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            Username = username;
            Password = password;
        }

        public string AuthenticateServiceKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}