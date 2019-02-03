using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiInitializeRequest
    {
        internal BankIdLoginApiInitializeRequest()
        {
            
        }


        public string PersonalIdentityNumber { get; set; }

        [Required]
        public string ReturnUrl { get; set; }

        [Required]
        public string LoginOptions { get; set; }
    }
}