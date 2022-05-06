using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;

public class BankIdLoginApiStatusRequest
{
    public BankIdLoginApiStatusRequest()
    {

    }

    [Required]
    public string? OrderRef { get; set; }

    [Required]
    public string? ReturnUrl { get; set; }

    [Required]
    public string? LoginOptions { get; set; }

    public int AutoStartAttempts { get; set; }
}