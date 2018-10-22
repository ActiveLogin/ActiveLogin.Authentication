using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdState
    {
        public BankIdState(AuthenticationProperties authenticationProperties)
        {
            AuthenticationProperties = authenticationProperties;
        }

        public AuthenticationProperties AuthenticationProperties { get; }
    }
}