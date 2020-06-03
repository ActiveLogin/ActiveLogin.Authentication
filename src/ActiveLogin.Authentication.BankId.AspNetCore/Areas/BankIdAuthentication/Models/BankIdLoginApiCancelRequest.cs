using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiCancelRequest
    {
        internal BankIdLoginApiCancelRequest()
        {

        }

        [Required]
        public string? LoginOptions { get; set; }

        [Required]
        public string? OrderRef { get; set; }

        public string? CancelReturnUrl { get; set; }
    }
}
