using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiStatusRequest
    {
        internal BankIdLoginApiStatusRequest()
        {
            
        }

        [Required]
        public string OrderRef { get; set; }
        public string ReturnUrl { get; set; }
        public string LoginOptions { get; set; }
    }
}