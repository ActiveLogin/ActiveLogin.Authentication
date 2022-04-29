using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiInitializeRequest
    {
        public BankIdLoginApiInitializeRequest()
        {

        }

        [Required]
        public string? ReturnUrl { get; set; }

        [Required]
        public string? LoginOptions { get; set; }
    }
}
