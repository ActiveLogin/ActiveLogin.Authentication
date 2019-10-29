using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiCancelRequest
    {
        internal BankIdLoginApiCancelRequest()
        {

        }

        [Required]
        public string OrderRef { get; set; }

    }
}
