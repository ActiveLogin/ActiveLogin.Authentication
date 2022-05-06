using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiApiInitializeRequest
{
    public BankIdUiApiInitializeRequest()
    {

    }

    [Required]
    public string? ReturnUrl { get; set; }

    [Required]
    public string? LoginOptions { get; set; }
}
