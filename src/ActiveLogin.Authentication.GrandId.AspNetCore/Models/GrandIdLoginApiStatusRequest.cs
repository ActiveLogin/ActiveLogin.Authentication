using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    public class GrandIdLoginApiStatusRequest
    {
        [Required]
        public string OrderRef { get; set; }
        public string ReturnUrl { get; set; }
    }
}