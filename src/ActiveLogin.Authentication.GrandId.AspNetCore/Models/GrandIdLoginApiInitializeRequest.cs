using ActiveLogin.Identity.Swedish.AspNetCore.Validation;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    public class GrandIdLoginApiInitializeRequest
    {
        public string ReturnUrl { get; set; }

        [SwedishPersonalIdentityNumber]
        public string PersonalIdentityNumber { get; set; }
    }
}