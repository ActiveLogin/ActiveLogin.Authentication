using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdState
    {
        public AuthenticationProperties AuthenticationProperties { get; set; }
    }
}