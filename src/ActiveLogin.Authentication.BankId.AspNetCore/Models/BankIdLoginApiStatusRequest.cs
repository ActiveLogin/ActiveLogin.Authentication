using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiStatusRequest
    {
        [Required]
        public string OrderRef { get; set; }
        public string ReturnUrl { get; set; }
        public string LoginOptions { get; set; }
    }
}