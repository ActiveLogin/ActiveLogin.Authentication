using ActiveLogin.Identity.Swedish.AspNetCore.Validation;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiInitializeRequest
    {
        [SwedishPersonalIdentityNumber]
        public string PersonalIdentityNumber { get; set; }
    }
}