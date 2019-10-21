using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdCancelApiStatusRequest
    {
        internal BankIdCancelApiStatusRequest()
        {

        }

        [Required]
        public string OrderRef { get; set; }

    }
}
