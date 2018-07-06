using ActiveLogin.Identity.Swedish.AspNetCore.Validation;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiInitializeRequest
    {
        public string ReturnUrl { get; set; }

        [SwedishPersonalIdentityNumber]
        public string PersonalIdentityNumber { get; set; }
    }
}