using ActiveLogin.Authentication.GrandId.Api;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    public class GrandIdLoginApiSessionResponse
    {
        public string SessionId { get; set; }

        public UserAttributes UserAttributes { get; set; }
    }
}