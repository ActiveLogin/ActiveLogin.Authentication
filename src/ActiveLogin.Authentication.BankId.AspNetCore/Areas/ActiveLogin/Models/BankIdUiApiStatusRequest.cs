using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiApiStatusRequest
{
    public BankIdUiApiStatusRequest()
    {

    }

    [Required]
    public string? OrderRef { get; set; }

    [Required]
    public string? ReturnUrl { get; set; }

    public int AutoStartAttempts { get; set; }
}
