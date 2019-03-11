using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdState
    {
        internal BankIdState(AuthenticationProperties authenticationProperties)
        {
            AuthenticationProperties = authenticationProperties;
        }

        public AuthenticationProperties AuthenticationProperties { get; }
    }
}
